namespace Proiect_POO.Entities;
using Proiect_POO.ValueObjects;
//utilizator care doneaza

public sealed class Donator:User
{
    public Donator(UserId userId, string nume, Email email) : base(userId, nume, email) { }
}