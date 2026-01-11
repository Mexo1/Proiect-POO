namespace Proiect_POO.Services;
using Microsoft.Extensions.Logging;
using Proiect_POO.Exceptions;
using Proiect_POO.Aggregates;
using Proiect_POO.Entities;
using Proiect_POO.ValueObjects;
public class UserService
{
    private readonly ILogger<UserService> _logger;

    public UserService(ILogger<UserService> logger)
    {
        _logger = logger;
    }

    public IReadOnlyList<Campaign> TrackCampaign(UserId user, IEnumerable<Campaign> allcampaigns)
    {
        var selectedCampaigns=allcampaigns.Where(c => c.GetDonations().Any(d => d.UserId.Equals(user))).ToList().AsReadOnly();
        _logger.LogInformation("User contributed to a number of {Count} campaigns:",selectedCampaigns.Count);
        return selectedCampaigns;
    }

    public IReadOnlyList<Donator> TrackDonation(AdminONG adminOng, IEnumerable<User> allusers)
    {
        if(adminOng==null)
            throw new DomainException("Invalid Admin");
        var donors=allusers.OfType<Donator>().ToList().AsReadOnly();
        _logger.LogInformation("Admin {Name} viewed all donors,total donors {Count}",adminOng.Nume,donors.Count);
        return donors;
    }
}