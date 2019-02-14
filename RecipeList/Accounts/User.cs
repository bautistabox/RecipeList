using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace RecipeList.Accounts
{
    [Table("users")]
    public class User
    {
        [Column("id")] [Key] public int Id { get; set; }
        
        [Column("email")] public string Email { get; set; }

        [Column("username")] public string Username { get; set; }

        [Column("display_name")] public string DisplayName { get; set; }

        [Column("password")] public string Password { get; set; }

        [Column("registered_at")] public DateTime RegisteredAt { get; set; }

        [Column("last_login_at")] public DateTime? LastLoginAt { get; set; }
    }
}