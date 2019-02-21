using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeList.Accounts
{
    [Table("email_verification")]
    public class EmailVerification
    {
        [Column("id")] [Key] public int Id { get; set; }
        [Column("user_id")] public int UserId { get; set; }
        [Column("guid")] public Guid GuId { get; set; }
        [Column("is_verified")] public bool IsVerified { get; set; }
    }
}