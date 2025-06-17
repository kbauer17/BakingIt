using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BakingIt.Models
{
    public class Conversion
    {
        public int ConversionId { get; set; }

        [Display(Name = "Pantry Ingredient")]
        public int? IngredientId { get; set; }

        [Required(ErrorMessage = "Please enter the Conversion Name")]
        [Display(Name = "Conversion Name")]
        public string ConversionName { get; set; } = null!;
        public string? Notes { get; set; }

        [Display(Name = "Initial Measure")]
        public int FromMeasureId { get; set; }

        [Display(Name = "Final Measure")]
        public int ToMeasureId { get; set; }

        [Display(Name = "Conversion Factor")]
        public decimal ConversionFactor { get; set; }

        // Navigation properties
        public virtual Ingredient? Ingredient { get; set; }

        [ValidateNever]
        public virtual Measure FromMeasure { get; set; } = null!;
        [ValidateNever]
        public virtual Measure ToMeasure { get; set; } = null!;
    }
}
