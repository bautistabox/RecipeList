using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Drawing;
using Microsoft.AspNetCore.Http;

namespace RecipeList.Recipes
{
    public class Recipe
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Recipe Title")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Brief Description of Recipe")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Detailed List of Instructions")]
        public string Instruction { get; set; }

        [Required]
        [Display(Name = "Category of Recipe")]
        public int CategoryId { get; set; }

        [Display(Name = "Optional - Post a photo of your dish")]
        public IFormFile Photo { get; set; }

        public int UploaderId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}