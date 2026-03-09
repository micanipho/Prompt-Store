using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>EF Core implementation of IUserRepository backed by SQL Server.</summary>
public class EfUserRepository : IUserRepository
{
    private readonly ShoppingDbContext _context;

    public EfUserRepository(ShoppingDbContext context)
    {
        _context = context;
    }

    public void AddUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public User? GetUserByUsername(string username)
    {
        return _context.Users
            .FirstOrDefault(u => u.UserName.ToLower() == username.ToLower());
    }

    public bool UsernameExists(string username)
    {
        return _context.Users
            .Any(u => u.UserName.ToLower() == username.ToLower());
    }
}
