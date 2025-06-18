using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BakingIt.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }

        [Required(ErrorMessage = "Please enter the Recipe Name")]
        [Display(Name = "Recipe Name")]
        public required string RecipeName { get; set; }

        [Display(Name = "Instructions or Notes")]
        public string? Instructions { get; set; }

        [MinLength(1, ErrorMessage = "Please add at least one ingredient.")]
        public required IList<RecipeIngredient> RecipeIngredients { get; set; }

        // Constructor to enforce required properties
        [SetsRequiredMembers]
        public Recipe(string recipeName)
        {
            RecipeName = recipeName;
            RecipeIngredients = new List<RecipeIngredient>();
        }
    }
}
