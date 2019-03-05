using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeList.Recipe
{
    [Table("recipe_ratings")]
    public class RecipeRating
    {
        [Column("id")][Key] public int Id { get; set; }
        [Column("recipe_id")]public int RecipeId { get; set; }
        [Column("user_id")] public int UserId { get; set; }
        [Column("rating")] public int Rating { get; set; }
    }
}