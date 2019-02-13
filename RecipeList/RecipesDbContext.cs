using Microsoft.EntityFrameworkCore;
using RecipeList.Accounts;

namespace RecipeList
{
    public class RecipesDbContext : DbContext
    {
        public RecipesDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}