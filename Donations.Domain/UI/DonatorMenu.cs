using Microsoft.Extensions.Logging;
using Proiect_POO.Services;
using Proiect_POO.Entities;
using Proiect_POO.Aggregates;
using Proiect_POO.ValueObjects;

namespace Proiect_POO.UI;

public class DonatorMenu
{
    private readonly CampaignService _campaignService;
    private readonly DonationService _donationService;
    private readonly ILogger _logger;
    private readonly List<Campaign> _campaigns;
    private readonly List<User> _users;
    private readonly List<Donation> _donations;
    private readonly Donator _currentDonator;

    public DonatorMenu(
        CampaignService campaignService,
        DonationService donationService,
        ILogger logger,
        List<Campaign> campaigns,
        List<User> users,
        List<Donation> donations,
        Donator currentDonator)
    {
        _campaignService = campaignService;
        _donationService = donationService;
        _logger = logger;
        _campaigns = campaigns;
        _users = users;
        _donations = donations;
        _currentDonator = currentDonator;
    }

    public void Display()
    {
        bool inMenu = true;
        while (inMenu)
        {
            DisplayDonatorMenu();
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    BrowseCampaigns();
                    break;
                case "2":
                    MakeDonation();
                    break;
                case "3":
                    ViewMyDonations();
                    break;
                case "4":
                    inMenu = false;
                    break;
                default:
                    Console.WriteLine("⚠ Opțiune invalidă!");
                    break;
            }
        }
    }

    private void DisplayDonatorMenu()
    {
        Console.WriteLine("\n╔════════════════════════════════╗");
        Console.WriteLine("║     MENIU DONATOR               ║");
        Console.WriteLine("╠════════════════════════════════╣");
        Console.WriteLine("║ 1. Răsfoiește Campanii         ║");
        Console.WriteLine("║ 2. Fă o Donație                ║");
        Console.WriteLine("║ 3. Vezi Donațiile mele         ║");
        Console.WriteLine("║ 4. Ieșire                      ║");
        Console.WriteLine("╚════════════════════════════════╝");
        Console.Write("Alege opțiune: ");
    }

    private void BrowseCampaigns()
    {
        var activeCampaigns = _campaigns.Where(c => c.IsActive).ToList();
        
        if (activeCampaigns.Count == 0)
        {
            Console.WriteLine("\n⚠ Nu sunt campanii active!");
            return;
        }

        Console.WriteLine("\n╔════════════════════════════════════════════╗");
        Console.WriteLine("║        CAMPANII DISPONIBILE                ║");
        Console.WriteLine("╚════════════════════════════════════════════╝");

        for (int i = 0; i < activeCampaigns.Count; i++)
        {
            var campaign = activeCampaigns[i];
            decimal progress = campaign.TargetAmount > 0 
                ? (campaign.GetCurrentAmount() / campaign.TargetAmount) * 100 
                : 0;

            Console.WriteLine($"\n{i + 1}. 📋 {campaign.Title}");
            Console.WriteLine($"   Categorie: {campaign.Category}");
            Console.WriteLine($"   Țintă: {campaign.TargetAmount:C}");
            Console.WriteLine($"   Colectat: {campaign.GetCurrentAmount():C} ({progress:F0}%)");
            Console.WriteLine($"   Donații: {campaign.GetDonations().Count}");
        }
    }

    private void MakeDonation()
    {
        try
        {
            var activeCampaigns = _campaigns.Where(c => c.IsActive).ToList();
            
            if (activeCampaigns.Count == 0)
            {
                Console.WriteLine("\n⚠ Nu sunt campanii active!");
                return;
            }

            BrowseCampaigns();
            Console.Write("\nAlege numărul campaniei: ");
            
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > activeCampaigns.Count)
            {
                Console.WriteLine("⚠ Selecție invalidă!");
                return;
            }

            var campaign = activeCampaigns[choice - 1];

            Console.Write("Suma dorită: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("⚠ Sumă invalidă!");
                return;
            }

            Console.WriteLine("Tip donație: 1=Unic, 2=Recurring");
            Console.Write("Alege: ");
            string? typeChoice = Console.ReadLine();
            var donationType = typeChoice == "2" ? DonationType.Recurring : DonationType.OneTime;

            var donation = new Donation(
                new DonationId(Guid.NewGuid()),
                _currentDonator.Id,
                new Money(amount),
                campaign.Id,
                donationType
            );

            campaign.AddDonation(donation);
            _donations.Add(donation);

            _logger.LogInformation("Donație: {Amount}, Tip: {Type}, Campanie: {Campaign}", 
                amount, donationType, campaign.Title);
            
            Console.WriteLine($"✓ Donație de {amount:C} acceptată! Mulțumim! 🙏");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Eroare: {ex.Message}");
        }
    }

    private void ViewMyDonations()
    {
        var myDonations = _donations.Where(d => d.UserId.Equals(_currentDonator.Id)).ToList();
        
        Console.WriteLine($"\n--- DONAȚIILE MELE ({myDonations.Count}) ---");
        
        if (myDonations.Count == 0)
        {
            Console.WriteLine("Nu ai făcut nicio donație încă.");
            return;
        }

        decimal totalDonated = 0;
        foreach (var donation in myDonations)
        {
            var campaign = _campaigns.FirstOrDefault(c => c.Id.Equals(donation.CampaignId));
            Console.WriteLine($"\n💵 {donation.Amount.Amount:C}");
            Console.WriteLine($"   Tip: {donation.Type}");
            Console.WriteLine($"   Campanie: {campaign?.Title ?? "Necunoscută"}");
            totalDonated += donation.Amount.Amount;
        }

        Console.WriteLine($"\n📊 Total donat: {totalDonated:C}");
    }
}
