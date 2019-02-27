using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mime;
using System.Drawing;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace RecipeList.Recipes
{
    [Table("recipes")]
    public class Recipe
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Recipe Title")]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Brief Description of Recipe")]
        [Column("description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Detailed List of Instructions")]
        [Column("instruction")]
        public string Instruction { get; set; }

        [Required]
        [Display(Name = "Category of Recipe")]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Display(Name = "Optional - Post a photo of your dish")]
        [Column("photo")]
        public Byte[] Photo { get; set; }

        [Column("uploader_id")]
        public int UploaderId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}