namespace Proiect_POO.Entities;
using Proiect_POO.ValueObjects;

public class Donation
{
    public DonationId Id { get; }
    public UserId UserId { get; }
    public CampaignId CampaignId { get; }
    public Money Amount { get; }
    public DateTime CreateAt{ get; }
    public DonationType Type { get; }

    public Donation(DonationId id, UserId userId, Money amount,CampaignId campaignId,DonationType type)
    {
        Id = id;
        UserId = userId;
        Amount = amount;
        CreateAt = DateTime.Now;
        CampaignId = campaignId;
        Type = type;
    }
}
