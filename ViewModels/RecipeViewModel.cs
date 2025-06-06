using BakingIt.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BakingIt.ViewModels
{
    public class RecipeViewModel
    {
        // Our domain entity is included directly.
        public Recipe Recipe { get; set; } = new Recipe( "My New Recipe"){RecipeName = "My New Recipe"};

        // The measure select list comes from the MeasureService.
        public IEnumerable<SelectListItem> Measures { get; set; } = new List<SelectListItem>();
    }
}