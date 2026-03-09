namespace Application.Dtos;

/// <summary>Carries the credentials and role submitted during user registration.</summary>
public class RegisterUserRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Customer;
}