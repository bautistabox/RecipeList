using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeList.Recipe
{
    [Table("saved_recipes")]
    public class SavedRecipe
    {
        [Key][Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("recipe_id")]
        public int RecipeId { get; set; }
        [Column("saved_at")]
        public DateTime SavedAt { get; set; }
    }
}