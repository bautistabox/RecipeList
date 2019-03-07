using System.ComponentModel.DataAnnotations;

namespace RecipeList.Accounts
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        public string PasswordOne { get; set; }
        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        public string PasswordTwo { get; set; } 
    }
}