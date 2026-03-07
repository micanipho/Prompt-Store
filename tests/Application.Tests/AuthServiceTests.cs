using Application.Services;
using Application.Dtos;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Moq;

namespace Application.Tests;

/// <summary>Unit tests for AuthService covering registration, login, and all edge cases.</summary>
public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _authService = new AuthService(_repositoryMock.Object);
    }

    #region Register

    [Fact]
    public void Register_ValidCustomerRequest_AddsUserToRepository()
    {
        _repositoryMock.Setup(r => r.UsernameExists("john")).Returns(false);

        _authService.Register(new RegisterUserRequest
        {
            UserName = "john",
            Password = "pass123",
            Role = UserRole.Customer
        });

        _repositoryMock.Verify(r => r.AddUser(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public void Register_ValidAdminRequest_AddsAdministratorToRepository()
    {
        _repositoryMock.Setup(r => r.UsernameExists("adminUser")).Returns(false);

        _authService.Register(new RegisterUserRequest
        {
            UserName = "adminUser",
            Password = "admin123",
            Role = UserRole.Admin
        });

        _repositoryMock.Verify(r => r.AddUser(It.IsAny<Administrator>()), Times.Once);
    }

    [Fact]
    public void Register_EmptyUsername_ThrowsArgumentException()
    {
        var request = new RegisterUserRequest
        {
            UserName = "",
            Password = "pass123",
            Role = UserRole.Customer
        };

        Assert.Throws<ArgumentException>(() => _authService.Register(request));
    }

    [Fact]
    public void Register_WhitespaceUsername_ThrowsArgumentException()
    {
        var request = new RegisterUserRequest
        {
            UserName = "   ",
            Password = "pass123",
            Role = UserRole.Customer
        };

        Assert.Throws<ArgumentException>(() => _authService.Register(request));
    }

    [Fact]
    public void Register_EmptyPassword_ThrowsArgumentException()
    {
        var request = new RegisterUserRequest
        {
            UserName = "john",
            Password = "",
            Role = UserRole.Customer
        };

        Assert.Throws<ArgumentException>(() => _authService.Register(request));
    }

    [Fact]
    public void Register_WhitespacePassword_ThrowsArgumentException()
    {
        var request = new RegisterUserRequest
        {
            UserName = "john",
            Password = "   ",
            Role = UserRole.Customer
        };

        Assert.Throws<ArgumentException>(() => _authService.Register(request));
    }

    [Fact]
    public void Register_DuplicateUsername_ThrowsInvalidOperationException()
    {
        _repositoryMock.Setup(r => r.UsernameExists("john")).Returns(true);

        var request = new RegisterUserRequest
        {
            UserName = "john",
            Password = "pass123",
            Role = UserRole.Customer
        };

        Assert.Throws<InvalidOperationException>(() => _authService.Register(request));
    }

    [Fact]
    public void Register_DuplicateUsername_DoesNotAddUser()
    {
        _repositoryMock.Setup(r => r.UsernameExists("john")).Returns(true);

        var request = new RegisterUserRequest { UserName = "john", Password = "pass123", Role = UserRole.Customer };

        try { _authService.Register(request); } catch { }

        _repositoryMock.Verify(r => r.AddUser(It.IsAny<User>()), Times.Never);
    }

    #endregion

    #region Login

    [Fact]
    public void Login_ValidCredentials_ReturnsUser()
    {
        var customer = new Customer("john", "pass123");
        _repositoryMock.Setup(r => r.GetUserByUsername("john")).Returns(customer);

        var result = _authService.Login(new LoginRequest { UserName = "john", Password = "pass123" });

        Assert.Equal(customer, result);
    }

    [Fact]
    public void Login_EmptyUsername_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _authService.Login(new LoginRequest { UserName = "", Password = "pass123" }));
    }

    [Fact]
    public void Login_WhitespaceUsername_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _authService.Login(new LoginRequest { UserName = "  ", Password = "pass123" }));
    }

    [Fact]
    public void Login_EmptyPassword_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _authService.Login(new LoginRequest { UserName = "john", Password = "" }));
    }

    [Fact]
    public void Login_UnknownUsername_ThrowsUnauthorizedAccessException()
    {
        _repositoryMock.Setup(r => r.GetUserByUsername("unknown")).Returns((User?)null);

        Assert.Throws<UnauthorizedAccessException>(() =>
            _authService.Login(new LoginRequest { UserName = "unknown", Password = "pass123" }));
    }

    [Fact]
    public void Login_WrongPassword_ThrowsUnauthorizedAccessException()
    {
        var customer = new Customer("john", "correctPass");
        _repositoryMock.Setup(r => r.GetUserByUsername("john")).Returns(customer);

        Assert.Throws<UnauthorizedAccessException>(() =>
            _authService.Login(new LoginRequest { UserName = "john", Password = "wrongPass" }));
    }

    [Fact]
    public void Login_WrongPassword_ErrorMessageDoesNotRevealWhichFieldFailed()
    {
        _repositoryMock.Setup(r => r.GetUserByUsername("john")).Returns((User?)null);

        var ex = Assert.Throws<UnauthorizedAccessException>(() =>
            _authService.Login(new LoginRequest { UserName = "john", Password = "pass123" }));

        Assert.Equal("Invalid username or password.", ex.Message);
    }

    [Fact]
    public void Login_CorrectCredentials_ReturnsCorrectRole()
    {
        var admin = new Administrator("adminUser", "admin123");
        _repositoryMock.Setup(r => r.GetUserByUsername("adminUser")).Returns(admin);

        var result = _authService.Login(new LoginRequest { UserName = "adminUser", Password = "admin123" });

        Assert.Equal(UserRole.Admin, result.Role);
    }

    #endregion
}
