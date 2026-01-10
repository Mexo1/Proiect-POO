namespace Proiect_POO.Services;
using Microsoft.Extensions.Logging;
using Proiect_POO.Exceptions;
using Proiect_POO.Aggregates;
using Proiect_POO.Entities;
using Proiect_POO.ValueObjects;
public class DonationService
{
    private readonly ILogger<DonationService> _logger;
    private readonly List<Campaign> _campaigns; 
    
    public DonationService(ILogger<DonationService> logger, List<Campaign> campaigns)
    {
        _logger = logger;
        _campaigns = campaigns;
    }

    public void ProcessDonation(Donator donator, Campaign campaign, Money amount,DonationType type,int months = 1)
    {
        try
        {
            if (donator == null)
                throw new DomainException("Donator is null");
            if (campaign == null)
                throw new DomainException("Campaign is null");
            if (!campaign.IsActive)
                throw new DomainException("Campaign is not active");
            if (amount.Amount < 5)
                throw new DomainException("Amount must be greater than 5");

            if (type == DonationType.OneTime)
            {
                var donation = new Donation(DonationId.New(), donator.Id, amount, campaign.Id, type);
                campaign.AddDonation(donation);
                _logger.LogInformation("A {Type} donation {DonationId} of {Amount} by {User} for {Campaign} has been successfully processed",type, donation.Id, amount.Amount, donator.Nume, campaign.Title);
                SendThankYou(donator, amount, campaign);
            }
            else if (type == DonationType.Recurring)
            {
                for (int i = 0; i < months; i++)
                {
                    var donation = new Donation(DonationId.New(), donator.Id, amount, campaign.Id, type);
                    campaign.AddDonation(donation);
                    _logger.LogInformation("A {Type} donation {DonationId} of {Amount} by {User} for {Campaign} has been successfully processed",type, donation.Id, amount.Amount, donator.Nume, campaign.Title);
                    SendThankYou(donator, amount, campaign);
                }
                
            }
        }
        catch (DomainException ex)
        {
            _logger.LogWarning("Error processing donation {message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error processing donation {message}", ex.Message);
            throw;
        }
    }
    public void SendThankYou(Donator donator, Money amount, Campaign campaign)
    {
        if (donator == null)
            throw new DomainException("Invalid donor");
        
        _logger.LogInformation(
            "Thank you {DonorName} for your donation of {Amount} to campaign {CampaignTitle}",
            donator.Nume,
            amount.Amount,
            campaign.Title
        );
    }


    public IReadOnlyList<Donation> GetAllDonations()
    {
        var allDonations=_campaigns.SelectMany(c => c.GetDonations()).ToList().AsReadOnly();
        _logger.LogInformation("Admin viewed all donations. Total count: {Count}",allDonations.Count);
        return allDonations;

    }

    public IReadOnlyList<Donation> GetDonationHistorybyDonor(Donator donor)
    {
        if(donor==null)
            throw new DomainException("Invalid donor");
        var history=_campaigns.SelectMany(c => c.GetDonations()).Where(d => d.UserId.Equals(donor.Id)).ToList().AsReadOnly();
        _logger.LogInformation("The history has been accesed for donor {User}.Total donations {Count}", donor.Nume, history.Count);
        return history;
    }
}