using Microsoft.EntityFrameworkCore;
using RecipeList.Accounts;
using RecipeList.Ingredient;

namespace RecipeList
{
    public class RecipesDbContext : DbContext
    {
        public RecipesDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Ingredient.Ingredient> Ingredients { get; set; }
    }
}