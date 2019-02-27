using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeList.Recipe
{
    [Table("recipe_ingredients")]
    public class RecipeIngredients
    {
        [Key]
        [Column("ingredient_id")]
        public int IngredientId { get; set; }
        [Key]
        [Column("recipe_id")]
        public int RecipeId { get; set; }
        [Column("amount")]
        public string Amount { get; set; }
        [Column("amount_type")]
        public string AmountType { get; set; }
    }
}