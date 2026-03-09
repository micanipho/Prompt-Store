namespace Infrastructure.Tests;

/// <summary>Integration tests for EfUserRepository using EF Core InMemory provider.</summary>
public class EfUserRepositoryTests : IDisposable
{
    private readonly ShoppingDbContext _context;
    private readonly EfUserRepository _repository;

    public EfUserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ShoppingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ShoppingDbContext(options);
        _repository = new EfUserRepository(_context);

        // Seed default admin
        _context.Users.Add(new Administrator("admin", "admin123"));
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    #region Constructor / Seeding

    [Fact]
    public void Constructor_SeedsDefaultAdmin()
    {
        var admin = _repository.GetUserByUsername("admin");

        Assert.NotNull(admin);
        Assert.Equal(UserRole.Admin, admin.Role);
    }

    #endregion

    #region AddUser

    [Fact]
    public void AddUser_ValidUser_UserCanBeRetrieved()
    {
        var customer = new Customer("john", "pass123");

        _repository.AddUser(customer);

        Assert.NotNull(_repository.GetUserByUsername("john"));
    }

    [Fact]
    public void AddUser_MultipleUsers_AllCanBeRetrieved()
    {
        _repository.AddUser(new Customer("alice", "pass1"));
        _repository.AddUser(new Customer("bob", "pass2"));

        Assert.NotNull(_repository.GetUserByUsername("alice"));
        Assert.NotNull(_repository.GetUserByUsername("bob"));
    }

    #endregion

    #region GetUserByUsername

    [Fact]
    public void GetUserByUsername_ExistingUser_ReturnsUser()
    {
        var customer = new Customer("jane", "pass123");
        _repository.AddUser(customer);

        var result = _repository.GetUserByUsername("jane");

        Assert.NotNull(result);
        Assert.Equal("jane", result.UserName);
    }

    [Fact]
    public void GetUserByUsername_NonExistentUser_ReturnsNull()
    {
        var result = _repository.GetUserByUsername("nobody");

        Assert.Null(result);
    }

    [Fact]
    public void GetUserByUsername_IsCaseInsensitive()
    {
        _repository.AddUser(new Customer("John", "pass123"));

        Assert.NotNull(_repository.GetUserByUsername("john"));
        Assert.NotNull(_repository.GetUserByUsername("JOHN"));
        Assert.NotNull(_repository.GetUserByUsername("JoHn"));
    }

    [Fact]
    public void GetUserByUsername_ReturnsCorrectUserType_Customer()
    {
        _repository.AddUser(new Customer("customerUser", "pass123"));

        var result = _repository.GetUserByUsername("customerUser");

        Assert.IsType<Customer>(result);
    }

    [Fact]
    public void GetUserByUsername_ReturnsCorrectUserType_Administrator()
    {
        var result = _repository.GetUserByUsername("admin");

        Assert.IsType<Administrator>(result);
    }

    #endregion

    #region UsernameExists

    [Fact]
    public void UsernameExists_ExistingUsername_ReturnsTrue()
    {
        _repository.AddUser(new Customer("jane", "pass123"));

        Assert.True(_repository.UsernameExists("jane"));
    }

    [Fact]
    public void UsernameExists_NonExistentUsername_ReturnsFalse()
    {
        Assert.False(_repository.UsernameExists("nobody"));
    }

    [Fact]
    public void UsernameExists_IsCaseInsensitive()
    {
        _repository.AddUser(new Customer("Jane", "pass123"));

        Assert.True(_repository.UsernameExists("jane"));
        Assert.True(_repository.UsernameExists("JANE"));
    }

    [Fact]
    public void UsernameExists_DefaultAdminExists()
    {
        Assert.True(_repository.UsernameExists("admin"));
    }

    #endregion
}
