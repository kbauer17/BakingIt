using BakingIt.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class BakingItContext : IdentityDbContext<ApplicationUser>
{
    public BakingItContext(DbContextOptions<BakingItContext> options)
        : base(options){}

    // Define DbSets for your database tables
    // public DbSet<YourModel> YourTableName { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
    public DbSet<Conversion> Conversions { get; set; }
    public DbSet<Measure> Measures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Define Identity's tables
        base.OnModelCreating(modelBuilder);

        // Define primary keys
        modelBuilder.Entity<Ingredient>().HasKey(i => i.IngredientId);
        modelBuilder.Entity<Recipe>().HasKey(r => r.RecipeId);
        modelBuilder.Entity<Conversion>().HasKey(c => c.ConversionId);
        modelBuilder.Entity<Measure>().HasKey(m => m.MeasureId);

        modelBuilder.Entity<Ingredient>()
            .HasOne(i => i.Measure)
            .WithMany() // assuming one measure can be associated with many ingredients
            .HasForeignKey(i => i.MeasureId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent accidental deletion of related Measures
        
        modelBuilder.Entity<RecipeIngredient>()
            .HasKey(ri => ri.RecipeIngredientId);   // singular primary key instead of composite key

        // Define relationships
        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Recipe)    // one RecipeIngredient belongs to one Recipe
            .WithMany(r => r.RecipeIngredients) // the Recipe entity must have a navigation property called RecipeIngredients (List<RecipeIngredient>) as this property allows EF Core to establish the relationship, and queries can easily traverse from Recipe to its RecipeIngredients.  If didn't need this, then just use .WithMany() to establish the manmy-to-one relationship without direct navigation from Recipe to RecipeIngredients
            .HasForeignKey(ri => ri.RecipeId);

        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Ingredient)
            .WithMany(i => i.RecipeIngredients)
            .HasForeignKey(ri => ri.IngredientId);

        modelBuilder.Entity<Conversion>()
            .HasOne(c => c.Ingredient)
            .WithMany(i => i.Conversions)
            .HasForeignKey(c => c.IngredientId);

        modelBuilder.Entity<Measure>()
            .HasIndex(m => m.MeasureName)
            .IsUnique();    // this will throw a DbUpdateException if try to save a duplicate

        modelBuilder.Entity<Measure>()
            .HasIndex(m => m.MeasureAbbreviation)
            .IsUnique();
    }
}