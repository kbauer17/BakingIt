using System.ComponentModel.DataAnnotations;

namespace BakingIt.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }
        
        [Required]
        [Display(Name = "Ingredient Name")]
        public required string IngredientName { get; set; }

        [Required]
        [Display(Name = "Purchase Unit")]
        public required string PurchaseUnit { get; set; }

        [Required]
        [Display(Name = "Purchase Quantity")]
        public required decimal PurchaseQuantity { get; set; }

        [Required]
        [Display(Name = "Purchase Cost")]
        public required decimal PurchaseCost { get; set; }

        // Navigation properties
        public ICollection<Conversion> Conversions { get; set; } = null!;
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = null!;
    }
}
