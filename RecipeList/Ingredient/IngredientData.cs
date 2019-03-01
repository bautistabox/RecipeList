using System.Collections.Generic;

namespace RecipeList.Ingredient
{
    public class IngredientData
    {
        public List<IngredientInfo> IngredientList { get; set; }
    }

    public class IngredientInfo
    {
        public string Ingredient { get; set; }
        public string Amount { get; set; }
        public string AmountType { get; set; }
    }
}