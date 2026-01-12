namespace Proiect_POO.Entities;
using Proiect_POO.ValueObjects;
//utilizator specializat

public sealed class AdminONG:User 
{
    public string OrganizationName { get; }

    public AdminONG(UserId id, string nume, Email email, string organizationName) : base(id, nume, email)
    {
        OrganizationName = organizationName;
    }
}