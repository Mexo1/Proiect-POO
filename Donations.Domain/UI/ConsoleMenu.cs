using Microsoft.Extensions.Logging;
using Proiect_POO.Services;
using Proiect_POO.Entities;
using Proiect_POO.Aggregates;
using Proiect_POO.ValueObjects;
using Proiect_POO.Persistence;

namespace Proiect_POO.UI;

public class ConsoleMenu
{
    private readonly UserService _userService;
    private readonly CampaignService _campaignService;
    private readonly DonationService _donationService;
    private readonly AuthenticationService _authService;
    private readonly ReportService _reportService;
    private readonly JsonStorageService _storageService;
    private readonly ILogger<ConsoleMenu> _logger;

    private readonly List<Campaign> _campaigns;
    private readonly List<User> _users;
    private readonly List<Donation> _donations;

    public ConsoleMenu(
        UserService userService,
        CampaignService campaignService,
        DonationService donationService,
        AuthenticationService authService,
        ReportService reportService,
        JsonStorageService storageService,
        ILogger<ConsoleMenu> logger,
        List<Campaign> campaigns,
        List<User> users,
        List<Donation> donations)
    {
        _userService = userService;
        _campaignService = campaignService;
        _donationService = donationService;
        _authService = authService;
        _reportService = reportService;
        _storageService = storageService;
        _logger = logger;
        _campaigns = campaigns;
        _users = users;
        _donations = donations;
    }

    public void Start()
    {
        _logger.LogInformation("=== Aplicația Donatii ONG ===");
        LoadData();
        
        bool running = true;
        while (running)
        {
            DisplayMainMenu();
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AuthenticateUser();
                    break;
                case "2":
                    RegisterUser();
                    break;
                case "3":
                    ViewAllCampaigns();
                    break;
                case "4":
                    SaveData();
                    break;
                case "5":
                    running = false;
                    Console.WriteLine("Au revoir!");
                    break;
                default:
                    Console.WriteLine("⚠ Opțiune invalidă!");
                    break;
            }
        }
    }

    private void DisplayMainMenu()
    {
        Console.WriteLine("\n╔════════════════════════════════╗");
        Console.WriteLine("║     MENIU PRINCIPAL             ║");
        Console.WriteLine("╠════════════════════════════════╣");
        Console.WriteLine("║ 1. Autentificare               ║");
        Console.WriteLine("║ 2. Înregistrare                ║");
        Console.WriteLine("║ 3. Vezi Campanii               ║");
        Console.WriteLine("║ 4. Salvează date               ║");
        Console.WriteLine("║ 5. Ieșire                      ║");
        Console.WriteLine("╚════════════════════════════════╝");
        Console.Write("Alege opțiune: ");
    }

    private void AuthenticateUser()
    {
        Console.Write("Email: ");
        string? email = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("⚠ Email invalid!");
            return;
        }

        try
        {
            var user = _authService.Authenticate(email, _users);
            Console.WriteLine($"✓ Binevenit, {user.Nume}!");

            if (user is AdminONG admin)
            {
                var adminMenu = new AdminMenu(_userService, _campaignService, _donationService, _reportService, 
                    _logger, _campaigns, _users, _donations);
                adminMenu.Display();
            }
            else if (user is Donator donator)
            {
                var donatorMenu = new DonatorMenu(_campaignService, _donationService, 
                    _logger, _campaigns, _users, _donations, donator);
                donatorMenu.Display();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Eroare la autentificare: {ex.Message}");
        }
    }

    private void RegisterUser()
    {
        Console.WriteLine("\n--- ÎNREGISTRARE ---");
        Console.Write("Nume: ");
        string? name = Console.ReadLine();
        Console.Write("Email: ");
        string? email = Console.ReadLine();
        Console.WriteLine("Tip utilizator: 1=Admin, 2=Donator");
        Console.Write("Alege: ");
        string? type = Console.ReadLine();

        try
        {
            var emailObj = new Email(email!);
            var userId = new UserId(Guid.NewGuid());

            User newUser = type == "1"
                ? new AdminONG(userId, name!, emailObj, "ONG")
                : new Donator(userId, name!, emailObj);

            _users.Add(newUser);
            Console.WriteLine("✓ Utilizator înregistrat cu succes!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Eroare: {ex.Message}");
        }
    }

    private void ViewAllCampaigns()
    {
        Console.WriteLine("\n--- CAMPANII ACTIVE ---");
        var activeCampaigns = _campaigns.Where(c => c.IsActive).ToList();
        
        if (activeCampaigns.Count == 0)
        {
            Console.WriteLine("Nu sunt campanii active.");
            return;
        }

        foreach (var campaign in activeCampaigns)
        {
            Console.WriteLine($"\n📋 {campaign.Title}");
            Console.WriteLine($"   Țintă: {campaign.TargetAmount:C} | Colectat: {campaign.GetCurrentAmount():C}");
            Console.WriteLine($"   Donații: {campaign.GetDonations().Count}");
        }
    }

    private void LoadData()
    {
        var loadedCampaigns = _storageService.LoadCampaigns();
        var loadedUsers = _storageService.LoadUsers();
        var loadedDonations = _storageService.LoadDonations();

        if (loadedCampaigns != null)
        {
            _campaigns.Clear();
            _campaigns.AddRange(loadedCampaigns);
        }
        if (loadedUsers != null)
        {
            _users.Clear();
            _users.AddRange(loadedUsers);
        }
        if (loadedDonations != null)
        {
            _donations.Clear();
            _donations.AddRange(loadedDonations);
        }
        _logger.LogInformation("Datele au fost încărcate din fișierele JSON");
    }

    private void SaveData()
    {
        try
        {
            _storageService.SaveCampaigns(_campaigns);
            _storageService.SaveUsers(_users);
            _storageService.SaveDonations(_donations);
            Console.WriteLine("✓ Datele au fost salvate cu succes!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Eroare la salvare: {ex.Message}");
        }
    }
}
