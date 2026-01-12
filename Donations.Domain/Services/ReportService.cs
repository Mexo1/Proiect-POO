using System.Diagnostics;

namespace Proiect_POO.Services;
using Microsoft.Extensions.Logging;
using Proiect_POO.Exceptions;
using Proiect_POO.Aggregates;
using Proiect_POO.Entities;
using Proiect_POO.ValueObjects;

public class ReportService
{
    private readonly ILogger<ReportService> _logger;
    public ReportService(ILogger<ReportService> logger)
    {
        _logger = logger;
    }

    public decimal GetSumByCategory(Category category, IEnumerable<Campaign> allcampaigns)
    {
        var filtredCampaigns=allcampaigns.Where(c => c.Category==category);
        decimal sum=0;
        foreach (var campaign in filtredCampaigns)
        {
            sum += campaign.GetCurrentAmount();
        }
        _logger.LogInformation("Total sum of {Sum} donated for campaigns under the category {Category}",sum, category);
        return sum;
    }

    private string ImpactDescription(Category category,decimal amount)
    {
        int buc = 0;
        string description = "";
        switch (category)
        {
            case Category.Education:
            {
                buc=(int)(amount/100);
                description = $"With donations for education, we provided {buc} school supplies and backpacks to children.";
            }
                break;
            case Category.Health:
            {
                buc=(int)(amount/50);
                description = $"With health funds, we have provided {buc} of vaccines and medicines.";
            } 
                break;
            case Category.Environment:
            {
                buc=(int)(amount/200);
                description = $"With the environmental donations, we planted {buc} trees and cleaned up parks.";
            } 
                break;
            case Category.Social:
            {
                buc=(int)(amount/85);
                description = $"With social funds, we supported {buc} vulnerable families and individuals.";
            } 
                break;
            default:
                return "There is no impact information for this category.";
        }
        return description;
    }

    public string ImpactReport(Campaign campaign)
    {
        if(campaign==null)
            throw new DomainException("Invalid campaign");
        decimal impactSum=campaign.GetCurrentAmount();
        string impact=ImpactDescription(campaign.Category,impactSum);
        _logger.LogInformation("Impact report has been succesfuly made");
        return impact;
    }

    public string GetDocumentConfiguration(Donator donor, Donation donation, Campaign campaign)
    {
        if(donor==null)
            throw new DomainException("Invalid donor");
        if(donation==null)
            throw new DomainException("Invalid donation");
        if(campaign==null)
            throw new DomainException("Invalid campaign");
        string document=
            $"~~~OFFICIAL DONATION CONFIRMATION~~~\n" +
            $"Donor: {donor.Nume}\n" +
            $"Donor Email: {donor.Email}\n" + 
            $"Donation ID: {donation.Id}\n" +
            $"Campaign: {campaign.Title}\n" +
            $"Amount: {donation.Amount.Amount} RON\n" +
            $"Date: {donation.CreateAt}\n" +
            $"Thank you for your support!\n";
        
        _logger.LogInformation("Donation confirmation generated for donor {DonorName} for campaign {CampaignTitle}", donor.Nume, campaign.Title);

        return document;
            
            
    }
}