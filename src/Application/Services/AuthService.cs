using Domain.Factories;

namespace Application.Services;

/// <summary>Handles user registration and authentication.</summary>
public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserFactory _userFactory;

    public AuthService(IUserRepository userRepository, IUserFactory userFactory)
    {
        _userRepository = userRepository;
        _userFactory = userFactory;
    }

    /// <summary>Registers a new user. Throws if input is empty or username is already taken.</summary>
    public void Register(RegisterUserRequest request)
    {
        Guard.Against.NullOrWhiteSpace(request.UserName, message: "Username cannot be empty.");
        Guard.Against.NullOrWhiteSpace(request.Password, message: "Password cannot be empty.");

        if (_userRepository.UsernameExists(request.UserName))
            throw new InvalidOperationException("Username already exists.");

        var user = _userFactory.CreateUser(request.UserName, request.Password, request.Role);

        _userRepository.AddUser(user);
    }

    /// <summary>Validates credentials and returns the authenticated user. Throws if credentials are invalid.</summary>
    public User Login(LoginRequest request)
    {
        Guard.Against.NullOrWhiteSpace(request.UserName, message: "Username cannot be empty.");
        Guard.Against.NullOrWhiteSpace(request.Password, message: "Password cannot be empty.");

        var user = _userRepository.GetUserByUsername(request.UserName)
            ?? throw new UnauthorizedAccessException("Invalid username or password.");

        if (user.Password != request.Password)
            throw new UnauthorizedAccessException("Invalid username or password.");

        return user;
    }
}