using Microsoft.AspNetCore.Mvc.Rendering;

namespace BakingIt.Services
{
    public interface IRecipeCalculationService
    {
        Task<decimal> CalculateRecipeCostAsync(int recipeId);
    }
}