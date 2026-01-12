using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Proiect_POO.Services;
using Proiect_POO.UI;
using Proiect_POO.Persistence;
using Proiect_POO.Aggregates;
using Proiect_POO.Entities;

// Inițializează listele de date
var campaigns = new List<Campaign>();
var users = new List<User>();
var donations = new List<Donation>();

// Configurare Dependency Injection
var services = new ServiceCollection();

// Înregistrare Logger
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

// Înregistrare listele de date (vor fi populate la runtime)
services.AddSingleton(campaigns);
services.AddSingleton(users);
services.AddSingleton(donations);

// Înregistrare Servicii
services.AddSingleton(sp => new UserService(sp.GetRequiredService<ILogger<UserService>>()));
services.AddSingleton(sp => new CampaignService(sp.GetRequiredService<ILogger<CampaignService>>(), campaigns));
services.AddSingleton(sp => new DonationService(sp.GetRequiredService<ILogger<DonationService>>(), campaigns));
services.AddSingleton(sp => new AuthenticationService(sp.GetRequiredService<ILogger<AuthenticationService>>()));
services.AddSingleton(sp => new ReportService(sp.GetRequiredService<ILogger<ReportService>>()));
services.AddSingleton<JsonStorageService>();
services.AddSingleton<ConsoleMenu>();

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Inițializare și rulare aplicație
var menu = serviceProvider.GetRequiredService<ConsoleMenu>();
menu.Start();
