namespace Proiect_POO.ValueObjects;

public readonly struct UserId(Guid Value)
{
    public static UserId New()=>new UserId(Guid.NewGuid());
}