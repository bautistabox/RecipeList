using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeList.Accounts
{
    [Table("unique_identifiers")]
    public class UniqueIdentifiers
    {
        [Column("id")] [Key] public int Id { get; set; }
        [Column("user_id")] public int UserId { get; set; }
        [Column("unique_id")] public Guid UniqueId { get; set; }
        [Column("is_verified")] public bool IsVerified { get; set; }
    }
}