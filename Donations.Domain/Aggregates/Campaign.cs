namespace Proiect_POO.Aggregates;
using Proiect_POO.Entities;
using Proiect_POO.ValueObjects;

public sealed class Campaign
{
    private readonly List<Donation> _donations;
    private readonly List<CampaignUpdate> _updates;
    public CampaignId Id { get;}
    public string Title { get; private set; }
    public decimal TargetAmount { get; }
    public bool IsActive { get; private set; } = true;
    public Category Category { get; }

    public Campaign(CampaignId id, string title, decimal targetAmount,Category category)
    {
        Id = id;
        Title = title;
        TargetAmount = targetAmount;
        Category = category;
        _donations = new List<Donation>();
        _updates = new List<CampaignUpdate>();
    }

    public void AddDonation(Donation donation)
    {
        if(!IsActive)
            throw new InvalidOperationException("Campaign is not active");
        _donations.Add(donation);
        if(GetCurrentAmount()>=TargetAmount)
            IsActive = false; //inchid campania daca s-a atins targetul
    }
    
    public void AddUpdate(CampaignUpdate update)
    {
        _updates.Add(update);
    }

    public decimal GetCurrentAmount() => _donations.Select(d => d.Amount.Amount).Sum();
    public IReadOnlyList<Donation> GetDonations() => _donations.AsReadOnly();
    public IReadOnlyList<CampaignUpdate> GetUpdates() => _updates.AsReadOnly();
    public void Close()
    {
        IsActive = false;
    }
}
