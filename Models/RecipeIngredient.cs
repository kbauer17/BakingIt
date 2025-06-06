using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BakingIt.Models
{
    public class RecipeIngredient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]   // Ensure auto-increment
        public int RecipeIngredientId { get; set; } // Primary Key
        public int RecipeId { get; set; } // Foreign Key linking to the Recipe table
        public int IngredientId { get; set; } // Foreign Key linking to the Ingredient table

        [Display(Name = "Quantity used in Recipe")]
        public decimal Quantity { get; set; } // Quantity needed in the recipe's unit

        [Display(Name = "Measure of the Quantity")]
        public int MeasureId { get; set; } // The measure for the quantity (e.g., cups, tablespoons, drops)

        // constructor to handle required properties
        public RecipeIngredient(){}

        // Navigation properties, which shouldn't be bound from form data
        [ValidateNever]
        public Recipe Recipe { get; set; } = null!; // Links to the Recipe entity
        [ValidateNever]
        public Ingredient Ingredient { get; set; } = null!; // Links to the Ingredient entity
        [ValidateNever]
        public Measure Measure { get; set; } = null!; // Links to the Measure entity
    }
}
