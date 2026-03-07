namespace Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public void Register(RegisterUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Username and password cannot be empty.");
        }
        if (_userRepository.UsernameExists(request.UserName))
        {
            throw new InvalidOperationException("Username already exists.");
        }

        User user = request.Role switch
        {
            UserRole.Customer => new Customer(request.UserName, request.Password),
            UserRole.Admin => new Administrator(request.UserName, request.Password),
            _ => throw new InvalidOperationException("Invalid user role.")
        };

        _userRepository.AddUser(user);
    }

    public User Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Username and password cannot be empty.");
        }
        var user = _userRepository.GetUserByUsername(request.UserName);
        if (user == null || user.Password != request.Password)
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        return user;
    }
}