namespace Domain.Interfaces;

public interface IUserRepository
{
    void AddUser(User user);
    User? GetUserByUsername(string username);
    bool UsernameExists(string username);
}