using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeList.Accounts
{
    [Table("user_bios")]
    public class UserBio
    {
        [Column("id")] [Key] public int Id { get; set; }
        [Column("user_id")] public int UserId { get; set; }
        [Column("bio")] public string Bio { get; set; }
    }
}