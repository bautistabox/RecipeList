using System.ComponentModel.DataAnnotations;

namespace RecipeList.Accounts
{
    public class ForgotCredentialsViewModel
    {
        [Required]
        public string Email { get; set; }
    }
}