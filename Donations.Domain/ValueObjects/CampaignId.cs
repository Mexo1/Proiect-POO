namespace Proiect_POO.ValueObjects;

public readonly struct CampaignId(Guid Value)
{ 
    public static CampaignId New()=>new CampaignId(Guid.NewGuid());
}
