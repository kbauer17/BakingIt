using BakingIt.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BakingIt.Services
{
    public class PantryIngredientService : IPantryIngredientService
    {
        private readonly IBakingItRepository<Ingredient> _ingredientRepository;

        public PantryIngredientService(IBakingItRepository<Ingredient> ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        public async Task<IEnumerable<SelectListItem>> GetPantryIngredientsSelectListAsync()
        {
            var ingredients = await _ingredientRepository.GetAllAsync();
            return ingredients.Select(i => new SelectListItem
            {
                Value = i.IngredientId.ToString(),
                Text = i.IngredientName
            });
        }
    }
}