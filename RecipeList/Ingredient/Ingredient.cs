using System;
using System.ComponentModel.DataAnnotations;

namespace RecipeList.Ingredient
{
    public class Ingredient
    {
        
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
//        [Required]
//        public DateTime CreatedAt { get; set; }
    }
}