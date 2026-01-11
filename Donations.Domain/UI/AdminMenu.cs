using Microsoft.Extensions.Logging;
using Proiect_POO.Services;
using Proiect_POO.Entities;
using Proiect_POO.Aggregates;
using Proiect_POO.ValueObjects;

namespace Proiect_POO.UI;

public class AdminMenu
{
    private readonly UserService _userService;
    private readonly CampaignService _campaignService;
    private readonly DonationService _donationService;
    private readonly ReportService _reportService;
    private readonly ILogger _logger;
    private readonly List<Campaign> _campaigns;
    private readonly List<User> _users;
    private readonly List<Donation> _donations;

    public AdminMenu(
        UserService userService,
        CampaignService campaignService,
        DonationService donationService,
        ReportService reportService,
        ILogger logger,
        List<Campaign> campaigns,
        List<User> users,
        List<Donation> donations)
    {
        _userService = userService;
        _campaignService = campaignService;
        _donationService = donationService;
        _reportService = reportService;
        _logger = logger;
        _campaigns = campaigns;
        _users = users;
        _donations = donations;
    }

    public void Display()
    {
        bool inMenu = true;
        while (inMenu)
        {
            DisplayAdminMenu();
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    CreateCampaign();
                    break;
                case "2":
                    ViewAllDonors();
                    break;
                case "3":
                    GenerateReport();
                    break;
                case "4":
                    CloseCampaign();
                    break;
                case "5":
                    inMenu = false;
                    break;
                default:
                    Console.WriteLine("⚠ Opțiune invalidă!");
                    break;
            }
        }
    }

    private void DisplayAdminMenu()
    {
        Console.WriteLine("\n╔════════════════════════════════╗");
        Console.WriteLine("║     MENIU ADMIN                 ║");
        Console.WriteLine("╠════════════════════════════════╣");
        Console.WriteLine("║ 1. Crează Campanie              ║");
        Console.WriteLine("║ 2. Vezi toți Donatorii          ║");
        Console.WriteLine("║ 3. Generează Raport             ║");
        Console.WriteLine("║ 4. Închide Campanie             ║");
        Console.WriteLine("║ 5. Ieșire                       ║");
        Console.WriteLine("╚════════════════════════════════╝");
        Console.Write("Alege opțiune: ");
    }

    private void CreateCampaign()
    {
        try
        {
            Console.WriteLine("\n--- CREARE CAMPANIE ---");
            Console.Write("Titlu: ");
            string? title = Console.ReadLine();
            
            Console.Write("Suma țintă: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal target))
            {
                Console.WriteLine("⚠ Suma invalidă!");
                return;
            }

            Console.WriteLine("Categorie: 1=Education, 2=Health, 3=Environment, 4=Social");
            Console.Write("Alege: ");
            string? catChoice = Console.ReadLine();
            
            var category = catChoice switch
            {
                "1" => Category.Education,
                "2" => Category.Health,
                "3" => Category.Environment,
                "4" => Category.Social,
                _ => Category.Social
            };

            var campaign = new Campaign(
                new CampaignId(Guid.NewGuid()),
                title!,
                target,
                category
            );

            _campaigns.Add(campaign);
            _logger.LogInformation("Campanie creată: {Title}", title);
            Console.WriteLine("✓ Campanie creată cu succes!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Eroare: {ex.Message}");
        }
    }

    private void ViewAllDonors()
    {
        try
        {
            var adminOng = _users.OfType<AdminONG>().FirstOrDefault();
            if (adminOng == null)
            {
                Console.WriteLine("⚠ Admin neidentificat!");
                return;
            }

            var donors = _userService.TrackDonation(adminOng, _users);
            
            Console.WriteLine($"\n--- TOȚI DONATORII ({donors.Count}) ---");
            foreach (var donor in donors)
            {
                Console.WriteLine($"👤 {donor.Nume} - {donor.Email.Value}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Eroare: {ex.Message}");
        }
    }

    private void GenerateReport()
    {
        try
        {
            Console.WriteLine("\n--- RAPORT GENERAL ---");
            Console.WriteLine($"📊 Total Campanii: {_campaigns.Count}");
            Console.WriteLine($"💰 Total Donații: {_donations.Count}");
            Console.WriteLine($"👥 Total Utilizatori: {_users.Count}");
            
            decimal totalAmount = _campaigns.Sum(c => c.GetCurrentAmount());
            Console.WriteLine($"💵 Sumă totală colectată: {totalAmount:C}");
            
            _logger.LogInformation("Raport generat");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Eroare: {ex.Message}");
        }
    }

    private void CloseCampaign()
    {
        try
        {
            Console.WriteLine("\n--- ÎNCHIDERE CAMPANIE ---");
            var activeCampaigns = _campaigns.Where(c => c.IsActive).ToList();
            
            if (activeCampaigns.Count == 0)
            {
                Console.WriteLine("Nu sunt campanii active de închis!");
                return;
            }

            for (int i = 0; i < activeCampaigns.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {activeCampaigns[i].Title}");
            }

            Console.Write("Alege numărul campaniei: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= activeCampaigns.Count)
            {
                activeCampaigns[choice - 1].Close();
                Console.WriteLine("✓ Campanie închisă cu succes!");
            }
            else
            {
                Console.WriteLine("⚠ Selecție invalidă!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Eroare: {ex.Message}");
        }
    }
}
