namespace RecipeList.Accounts
{
    public interface IPasswordHasher
    {
        string HashPassword(string userPassword);
        bool VerifyPassword(string userPassword, string dbPassword);
    }
}