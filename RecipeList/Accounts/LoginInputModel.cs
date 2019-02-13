using System.ComponentModel.DataAnnotations;

namespace RecipeList.Accounts
{
    public class LoginInputModel
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}