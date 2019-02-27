using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using RecipeList.Recipes;

namespace RecipeList.Recipe
{
    public class RecipeInfo
    {
        public Recipes.Recipe Recipe { get; set;  }
        public List<RecipeIngredients> RecipeIngredients { get; set; }
        public List<Ingredient.Ingredient> Ingredients { get; set; }
        public Category Category { get; set; }
        public List<string> IngredientsList { get; set; }
        
        public IFormFile Image { get; set; }
        }
}