namespace Proiect_POO.ValueObjects;
//gestionarea sumelor de bani
public sealed record Money
{
    public decimal Amount { get;}

    public Money(decimal amount)
    {
        if (amount <= 0)
            throw new Exception("Amount must be positive");
        Amount = amount;
    } 
    public static Money operator +(Money a, Money b)
        => new(a.Amount + b.Amount);
}