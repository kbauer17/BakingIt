using System.ComponentModel.DataAnnotations;

namespace BakingIt.Models
{
    public class Conversion
    {
        public int ConversionId { get; set; }
        public int IngredientId { get; set; }

        [Required]
        [Display(Name = "Beginning Unit")]
        public required string FromUnit { get; set; }

        [Required]
        [Display(Name = "Ending Unit")]
        public required string ToUnit { get; set; }
        public decimal ConversionFactor { get; set; }

        // Navigation properties
        public Ingredient Ingredient { get; set; } = null!;
    }
}
