using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using RecipeList.Ingredient;

namespace RecipeList.Recipes
{
    public class RecipeInputModel
    {
        public int RecipeId { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Recipe Title")]
        public string Name { get; set; }

        [Required]
        [MaxLength(1000000)]
        [Display(Name = "Brief Description of Recipe")]
        public string Description { get; set; }

        [Required]
        [MaxLength(1000000)]
        [Display(Name = "Detailed List of Instructions")]
        public string Instruction { get; set; }

        [Required]
        [Display(Name = "Category of Recipe")]
        public string Category { get; set; }

        [Display(Name = "Optional - Post a photo of your dish")]
        public IFormFile Photo { get; set; }

        public IList<IngredientInfo> Ingredients { get; set; }
    }
}