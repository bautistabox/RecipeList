using System.ComponentModel.DataAnnotations;

namespace RecipeList.Accounts
{
    public class RegisterInputModel
    {
        [Required]
        public string Email { get; set; }
        
        [Required]
        [MinLength(4)]
        [MaxLength(16)]
        public string Username { get; set; }
        
        [Required]
        [MinLength(4)]
        [MaxLength(16)]
        public string DisplayName { get; set; }
        
        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        public string Password { get; set; }
        
        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        public string ConfirmPassword { get; set; }
    }
}