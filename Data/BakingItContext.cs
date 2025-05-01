using BakingIt.Models;
using Microsoft.EntityFrameworkCore;

public class BakingItContext : DbContext
{
    public BakingItContext(DbContextOptions<BakingItContext> options)
        : base(options){}

    // Define DbSets for your database tables
    // public DbSet<YourModel> YourTableName { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
    public DbSet<Conversion> Conversions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Define primary keys
        modelBuilder.Entity<Ingredient>().HasKey(i => i.IngredientId);
        modelBuilder.Entity<Recipe>().HasKey(r => r.RecipeId);
        modelBuilder.Entity<Conversion>().HasKey(c => c.ConversionId);

        // Define composite key for RecipeIngredients (junction table)
        modelBuilder.Entity<RecipeIngredient>()
            .HasKey(ri => new { ri.RecipeId, ri.IngredientId });

        // Define relationships
        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Recipe)
            .WithMany(r => r.RecipeIngredients)
            .HasForeignKey(ri => ri.RecipeId);

        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Ingredient)
            .WithMany(i => i.RecipeIngredients)
            .HasForeignKey(ri => ri.IngredientId);

        modelBuilder.Entity<Conversion>()
            .HasOne(c => c.Ingredient)
            .WithMany(i => i.Conversions)
            .HasForeignKey(c => c.IngredientId);
    }
}