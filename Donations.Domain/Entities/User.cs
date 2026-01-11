using Proiect_POO.ValueObjects;

namespace Proiect_POO.Entities;

public abstract class User
{
    public UserId Id { get; }
    public string Nume { get; }
    public Email Email { get; }

    protected User(UserId id, string nume, Email email)
    {
        if (string.IsNullOrWhiteSpace(nume))
            throw new ArgumentException("Numele utilizatorului este obligatoriu!");
        
        Id = id;
        Nume = nume;
        Email = email;
    }
}