namespace Proiect_POO.Services;
using Microsoft.Extensions.Logging;
using Proiect_POO.Exceptions;
using Proiect_POO.Aggregates;
using Proiect_POO.Entities;
using Proiect_POO.ValueObjects;

public class CampaignService
{
    private readonly ILogger<CampaignService> _logger;
    private readonly List<Campaign> _campaigns;

    public CampaignService(ILogger<CampaignService> logger, List<Campaign> campaigns)
    {
        _logger = logger;
        _campaigns = campaigns;
    }

    public void CreateCampaign(string campaignName,decimal amount,Category category)
    {
        if (amount <= 0)
            throw new DomainException("Campaign amount must be greater than zero");
        
        var campaign=new Campaign(CampaignId.New(), campaignName, amount, category);
        _campaigns.Add(campaign);
        _logger.LogInformation("Campaign named {Name} has been succefuly created with amount target of {Amount}.",campaignName,amount);
        
    }

    public IReadOnlyList<Campaign> GetCampaignsByCategory(Category category)
    {
        var filtredCampaigns=_campaigns.Where(c => c.Category == category).ToList().AsReadOnly();
        _logger.LogInformation("Filtered campaigns by categorie {Category}.Total found {Count} ",category, filtredCampaigns.Count);
        return filtredCampaigns;
    }

    public IReadOnlyList<Campaign> FindCampaignsByState(bool state)
    {
        var stateC= (state ? "active" : "inactive");
        var findState=_campaigns.Where(c => c.IsActive == state).ToList().AsReadOnly();
        _logger.LogInformation("Filtred campaigns by state {State},total found{Count}",stateC,findState.Count);
        return findState;
    }
    
    public void MarkCampaign(AdminONG adminOng,Campaign campaign)
    {
        if(adminOng==null)
            throw new DomainException("Invalid Admin");
        if(campaign==null)
            throw new DomainException("Invalid campaign");
        if (!campaign.IsActive)
        {
            _logger.LogWarning("Trying to close an already inactive campaign {Title}",campaign.Title);
            return;
        }
        campaign.Close();
        _logger.LogInformation("Campaign {Title} has been closed", campaign.Title);
    }
    
    public void AddCampaignUpdate(AdminONG adminOng,Campaign campaign, string message)
    {
        if(adminOng==null)
            throw new DomainException("Invalid Admin");
        var update = new CampaignUpdate(message);
        campaign.AddUpdate(update);
        _logger.LogInformation("Admin {AdminName} added update to campaign {Title}: {Message}",adminOng.Nume, campaign.Title, message);
    }

    
}