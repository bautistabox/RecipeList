using Microsoft.EntityFrameworkCore;
using RecipeList.Accounts;
using RecipeList.Recipe;
using RecipeList.Shopping;

namespace RecipeList
{
    public class RecipesDbContext : DbContext
    {
        public RecipesDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Ingredient.Ingredient> Ingredients { get; set; }
        public DbSet<ShoppingList> Lists { get; set; }
        public DbSet<UniqueIdentifiers> UniqueIdentifiers { get; set; }
        public DbSet<ListItem> ListItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Recipe.Recipe> Recipes { get; set; }
        public DbSet<RecipeIngredients> RecipeIngredients { get; set; }
        public DbSet<UserBio> UserBios { get; set; }
        public DbSet<RecipeRating> RecipeRatings { get; set; }
        public DbSet<Comment.Comment> Comments { get; set; }
        public DbSet<SavedRecipe> SavedRecipes { get; set; }
 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecipeIngredients>()
                .HasKey(r => new {r.IngredientId, r.RecipeId});
        }
    }
}