using Microsoft.Extensions.Logging;
using Proiect_POO.Entities;
using Proiect_POO.Exceptions;

namespace Proiect_POO.Services;

public class AuthenticationService
{
    private readonly ILogger<AuthenticationService>  _logger;
    private User CurrentUser { get; set; }
    public AuthenticationService(ILogger<AuthenticationService> logger)
    {
        _logger = logger;
    }

    public User Authenticate(string email, IEnumerable<User> allusers)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.");
        
        User foundUser = null;
        foreach (var user in allusers)
        {
            if (user.Email.Value.ToLower() == email.ToLower())
            {
                foundUser = user;
                break;
            }
        }
        if (foundUser == null)
        {
            _logger.LogWarning("Authentification failed,email {Email} not found", email);
            throw new DomainException("User dosen't exist.Please register");
        }
        CurrentUser= foundUser;
        _logger.LogInformation("User successfully authenticated.Name{Name}:Role{Role}",foundUser.Nume,foundUser.GetType().Name);
        return foundUser;
    }

    public void Logout()
    {
        if (CurrentUser != null)
        {
            _logger.LogInformation("User {Name} successfully logged out.", CurrentUser.Nume);
            CurrentUser = null;
        }
        
    }
    public bool IsAdminLoggedIn() => CurrentUser is AdminONG;
}