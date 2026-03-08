namespace Application.Dtos;

/// <summary>Carries the credentials submitted during a login attempt.</summary>
public class LoginRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}