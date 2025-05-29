using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BakingIt.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }
        
        [Required(ErrorMessage = "Please enter the Ingredient Name")]
        [Display(Name = "Ingredient Name")]
        public required string IngredientName { get; set; }

        [Required(ErrorMessage = "Please enter the amount purchased")]
        [Display(Name = "Purchase Quantity")]
        public required decimal PurchaseQuantity { get; set; }

        [Required(ErrorMessage = "Please enter the amount paid when purchased")]
        [Display(Name = "Purchase Cost")]
        public required decimal PurchaseCost { get; set; }

        // Foreign Key to Measure
        [Required(ErrorMessage = "Please select the correct unit of measure for the purchase")]
        [Display(Name = "Purchase Measure or Unit")]
        public int MeasureId {get; set;}
        [ForeignKey("MeasureId")]
        public Measure? Measure {get; set;}

        // Navigation properties
        public ICollection<Conversion> Conversions { get; set; } = new List<Conversion>();
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    }
}
