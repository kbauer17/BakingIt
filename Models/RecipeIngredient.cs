using System.ComponentModel.DataAnnotations;

namespace BakingIt.Models
{
    public class RecipeIngredient
    {
        public int RecipeIngredientId { get; set; } // Primary Key
        public int RecipeId { get; set; } // Foreign Key linking to the Recipe table
        public int IngredientId { get; set; } // Foreign Key linking to the Ingredient table

        [Required]
        [Display(Name = "Quantity used in Recipe")]
        public required decimal Quantity { get; set; } // Quantity needed in the recipe's unit

        [Required]
        [Display(Name = "Units of the Quantity")]
        public required string Unit { get; set; } // The unit for the quantity (e.g., cups, tablespoons, drops)

        // Navigation properties
        public Recipe Recipe { get; set; } = null!; // Links to the Recipe entity
        public Ingredient Ingredient { get; set; } = null!; // Links to the Ingredient entity
    }
}
