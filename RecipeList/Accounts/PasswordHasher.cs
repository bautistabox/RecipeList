namespace RecipeList.Accounts
 {
     public class PasswordHasher : IPasswordHasher
     {
         public string HashPassword(string userPassword)
         {
             return BCrypt.Net.BCrypt.HashPassword(userPassword);
         }
 
         public bool VerifyPassword(string userPassword, string dbPassword)
         {
             return BCrypt.Net.BCrypt.Verify(userPassword, dbPassword);
         }
     }
 }