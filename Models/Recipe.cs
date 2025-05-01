using System.ComponentModel.DataAnnotations;

namespace BakingIt.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }

        [Required]
        [Display(Name = "Recipe Name")]
        public required string RecipeName { get; set; }
        public string? Instructions { get; set; }
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = null!;
    }
}
