public class Administrator : User
{
    public Administrator(string userName, string Password)
        : base(userName, UserRole.Admin, Password)
    {
        
    }
}