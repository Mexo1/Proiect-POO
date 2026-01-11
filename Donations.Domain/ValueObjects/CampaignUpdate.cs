namespace Proiect_POO.ValueObjects;

public sealed record CampaignUpdate
{
    public string Message { get; }
    public DateTime CreatedAt { get; }

    public CampaignUpdate(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty", nameof(message));

        Message = message;
        CreatedAt = DateTime.Now;
    }
}
