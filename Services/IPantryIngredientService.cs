using Microsoft.AspNetCore.Mvc.Rendering;

namespace BakingIt.Services
{
    public interface IPantryIngredientService
    {
        Task<IEnumerable<SelectListItem>> GetPantryIngredientsSelectListAsync();
    }
}