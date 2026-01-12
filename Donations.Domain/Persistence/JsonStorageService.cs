using System.Text.Json;
using Proiect_POO.Aggregates;
using Proiect_POO.Entities;

namespace Proiect_POO.Persistence;

public class JsonStorageService
{
    private readonly string _basePath;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public JsonStorageService(string basePath = "data")
    {
        _basePath = basePath;
        if (!Directory.Exists(_basePath))
            Directory.CreateDirectory(_basePath);
    }

    public void SaveCampaigns(List<Campaign> campaigns)
    {
        try
        {
            var path = Path.Combine(_basePath, "campaigns.json");
            var json = JsonSerializer.Serialize(campaigns, JsonOptions);
            File.WriteAllText(path, json);
            Console.WriteLine($"✓ Campanii salvate în {path}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Eroare la salvarea campaniilor", ex);
        }
    }

    public void SaveUsers(List<User> users)
    {
        try
        {
            var path = Path.Combine(_basePath, "users.json");
            var json = JsonSerializer.Serialize(users, JsonOptions);
            File.WriteAllText(path, json);
            Console.WriteLine($"✓ Utilizatori salvați în {path}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Eroare la salvarea utilizatorilor", ex);
        }
    }

    public void SaveDonations(List<Donation> donations)
    {
        try
        {
            var path = Path.Combine(_basePath, "donations.json");
            var json = JsonSerializer.Serialize(donations, JsonOptions);
            File.WriteAllText(path, json);
            Console.WriteLine($"✓ Donații salvate în {path}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Eroare la salvarea donațiilor", ex);
        }
    }

    public List<Campaign>? LoadCampaigns()
    {
        try
        {
            var path = Path.Combine(_basePath, "campaigns.json");
            if (!File.Exists(path))
                return new List<Campaign>();

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<Campaign>>(json, JsonOptions) ?? new List<Campaign>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Eroare la încărcarea campaniilor: {ex.Message}");
            return new List<Campaign>();
        }
    }

    public List<User>? LoadUsers()
    {
        try
        {
            var path = Path.Combine(_basePath, "users.json");
            if (!File.Exists(path))
                return new List<User>();

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<User>>(json, JsonOptions) ?? new List<User>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Eroare la încărcarea utilizatorilor: {ex.Message}");
            return new List<User>();
        }
    }

    public List<Donation>? LoadDonations()
    {
        try
        {
            var path = Path.Combine(_basePath, "donations.json");
            if (!File.Exists(path))
                return new List<Donation>();

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<Donation>>(json, JsonOptions) ?? new List<Donation>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Eroare la încărcarea donațiilor: {ex.Message}");
            return new List<Donation>();
        }
    }
}
