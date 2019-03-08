using System.Collections.Generic;

namespace RecipeList.Recipe
{
    public class RecipeInfo
    {
        public Recipe Recipe { get; set;  }
        public List<RecipeIngredients> RecipeIngredients { get; set; }
        public List<Ingredient.Ingredient> Ingredients { get; set; }
        public Category Category { get; set; }
        public List<Category> Categories { get; set; }
        public List<string> IngredientsList { get; set; }
        public int Rating { get; set; }
        public int RatingCount { get; set; }
        public int CurrentUser { get; set; }
        
        public string Image { get; set; }
        }
}