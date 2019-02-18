using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace RecipeList.Shopping
{
    [Table("lists")]
    public class ShoppingList
    {
        [Column("id")] [Key] public int Id { get; set; }

        [Column("name")] public string Name { get; set; }

        [Column("user_id")] public int UserId { get; set; }

        [Column("created_at")] public DateTime CreatedAt { get; set; }

        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
    }
}