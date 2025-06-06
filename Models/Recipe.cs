using System.ComponentModel.DataAnnotations;

namespace BakingIt.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }

        [Required(ErrorMessage = "Please enter the Recipe Name")]
        [Display(Name = "Recipe Name")]
        public required string RecipeName { get; init; }
        public string? Instructions { get; set; }
        public IList<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

        // Constructor to enforce required properties
        public Recipe(string recipeName)
        {
            RecipeName = recipeName;
        }
    }
}
