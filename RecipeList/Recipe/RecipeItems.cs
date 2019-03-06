using System;

namespace RecipeList.Recipe
{
    public class RecipeItems
    {
        public int RecipeId { get; set; }
        public string RecipeOwner { get; set; }
        public Int32 RecipeOwnerId { get; set; }
        public string RecipeName { get; set; }
        public string RecipeDescription { get; set; }
        public string RecipeDescShort { get; set; }
        public string RecipeInstruction { get; set; }
        public DateTime RecipeCreatedDate { get; set; }
        public DateTime RecipeUpdatedDate { get; set; }
        public byte[] RecipePhoto { get; set; }
        public string RecipePhoto64 { get; set; }
        public int RecipeRating { get; set; }
    }
}