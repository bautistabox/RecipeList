using System.Collections.Generic;
using RecipeList.Recipes;

namespace RecipeList.Accounts
{
    public class CategoriesList
    {
        public CategoriesList()
        {
            Categories = new List<Category>();
        }
        public List<Category> Categories { get; set; }
    }
}