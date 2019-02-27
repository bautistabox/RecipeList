using Microsoft.EntityFrameworkCore;
using RecipeList.Accounts;
using RecipeList.Ingredient;
using RecipeList.Recipe;
using RecipeList.Recipes;
using RecipeList.Shopping;

namespace RecipeList
{
    public class RecipesDbContext : DbContext
    {
        public RecipesDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Ingredient.Ingredient> Ingredients { get; set; }
        public DbSet<ShoppingList> Lists { get; set; }
        public DbSet<EmailVerification> EmailVerifications { get; set; }
        public DbSet<ListItem> ListItems { get; set; }
        public DbSet<ExpiredGuid> ExpiredGuids { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Recipes.Recipe> Recipes { get; set; }
        public DbSet<RecipeIngredients> RecipeIngredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecipeIngredients>()
                .HasKey(r => new {r.IngredientId, r.RecipeId});
        }
    }
    
    
}