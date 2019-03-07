using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeList.Recipe
{
    [Table("categories")]   
    public class Category
    {
        [Column("id")] [Key] public int Id { get; set; }
        [Column("name")] public string Name { get; set; }
    }
}