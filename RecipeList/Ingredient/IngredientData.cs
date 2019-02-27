using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

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