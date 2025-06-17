using BakingIt.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BakingIt.Services
{
    public class RecipeCalculationService : IRecipeCalculationService
    {
        private readonly IBakingItRepository<Conversion> _conversionRepository;
        private readonly IMeasureService _measureService;
        private readonly IPantryIngredientService _pantryIngredientService;
        private readonly IBakingItRepository<Recipe> _recipeRepository;

        public RecipeCalculationService(IBakingItRepository<Conversion> conversionRepository, IMeasureService measureService, IPantryIngredientService pantryIngredientService, IBakingItRepository<Recipe> recipeRepository)
        {
            _conversionRepository = conversionRepository;
            _measureService = measureService;
            _pantryIngredientService = pantryIngredientService;
            _recipeRepository = recipeRepository;
        }

        public async Task<decimal> CalculateRecipeCostAsync(int recipeId)
        {
            var recipe = await _recipeRepository.GetQueryable()
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Measure)
            .FirstOrDefaultAsync(r => r.RecipeId == recipeId);

            if (recipe == null)
                throw new Exception("Recipe not found.");

            decimal totalCost = 0;

            foreach (var recipeIngredient in recipe.RecipeIngredients)
            {
                var ingredient = recipeIngredient.Ingredient;

                // Prevent division by 0
                if (ingredient.PurchaseQuantity == 0) continue;

                // Look for a conversion that converts from the purchased measure to the recipe measure, with priority on an ingredient-specific conversion
                var conversion = _conversionRepository.GetQueryable()
                    .FirstOrDefault(c =>
                                (c.IngredientId == ingredient.IngredientId || c.IngredientId == null) &&
                                c.FromMeasureId == ingredient.MeasureId &&
                                c.ToMeasureId == recipeIngredient.MeasureId);

                // handle case of no conversion found:  for now, set equal to 1
                if (conversion == null)
                {
                    conversion = new Conversion { ConversionFactor = 1 };
                }

                // Calculate the cost per recipe unit
                // If 1 purchased unit converts to 'conversion.ConversionFactor' recipe units, then cost per recipe unit = (ingredient.UnitPrice/ingredient.PurchaseQuantity)/conversion.ConversionFactor
                decimal costPerRecipeUnit = (ingredient.PurchaseCost / ingredient.PurchaseQuantity) / conversion.ConversionFactor;

                decimal ingredientCost = recipeIngredient.Quantity * costPerRecipeUnit;
                totalCost += ingredientCost;
            }
            return totalCost;
        }
    }
}