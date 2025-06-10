using System.ComponentModel.DataAnnotations;

namespace BakingIt.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }

        [Display(Name = "Recipe Name")]
        public string? RecipeName { get; set; }
        [Display(Name = "Instructions or Notes")]
        public string? Instructions { get; set; }
        public IList<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

        // Constructor to enforce required properties
        public Recipe(string recipeName)
        {
            RecipeName = recipeName;
        }
    }
}
