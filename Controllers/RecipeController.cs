using BakingIt.Models;
using BakingIt.Services;
using BakingIt.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class RecipeController : Controller
{
    private readonly IBakingItRepository<Recipe> _recipeRepository;
    private readonly IBakingItRepository<Ingredient> _ingredientRepository;
    private readonly IBakingItRepository<RecipeIngredient> _recipeIngredientRepository;
    private readonly IMeasureService _measureService;
    private readonly IPantryIngredientService _pantryIngredientService;
    private readonly IRecipeCalculationService _recipeCalculationService;

    // Inject repository via constructor
    public RecipeController(IBakingItRepository<Recipe> recipeRepository, IBakingItRepository<Ingredient> ingredientRepository, IBakingItRepository<RecipeIngredient> recipeIngredientRepository, IMeasureService measureService, IPantryIngredientService pantryIngredientService, IRecipeCalculationService recipeCalculationService)
    {
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
        _recipeIngredientRepository = recipeIngredientRepository;
        _measureService = measureService;
        _pantryIngredientService = pantryIngredientService;
        _recipeCalculationService = recipeCalculationService;
    }

    /// <summary>
    ///     Display a table listing all existing Recipes by name
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> ViewRecipes()
    {
        var recipes = await _recipeRepository.GetAllAsync();
        return View(recipes);
    }

    // GET: Recipe Details
    public async Task<IActionResult> RecipeDetails(int id)
    {
        var recipe = await _recipeRepository.GetQueryable()
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Measure)
            .FirstOrDefaultAsync(r => r.RecipeId == id);
        if (recipe == null) return NotFound();

        return View(recipe);
    }

    /// <summary>
    ///     Create a new Recipe - Getter
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> CreateRecipe()
    {
        var model = new RecipeViewModel
        {
            Recipe = new Recipe(string.Empty)
        };

        // Populate the dropdown lists for the initial page load.
        ViewBag.PantryIngredients = await _pantryIngredientService.GetPantryIngredientsSelectListAsync();
        ViewBag.Measures = await _measureService.GetMeasuresSelectListAsync();

        return View(model);
    }

    /// <summary>
    ///     Create a new Recipe - Setter
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRecipe(RecipeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Repopulate the dropdown lists if returning due to validation errors.
            ViewBag.PantryIngredients = await _pantryIngredientService.GetPantryIngredientsSelectListAsync();
            ViewBag.Measures = await _measureService.GetMeasuresSelectListAsync();

            // Collect all error messages
            var allErrors = ModelState
                .SelectMany(x => x.Value!.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            // Attach errors to the ViewBag so they can be displayed in the view
            ViewBag.ModelErrors = allErrors;

            return View(model);
        }

        // In this approach the view model already contains the Recipe entity. 
        // Save it directly with your repository.
        await _recipeRepository.AddAsync(model.Recipe);

        return RedirectToAction("ViewRecipes");
    }

    /// <summary>
    ///     Getter for the _RecipeIngredientPartial.cshtml, called via AJAX within CreateRecipe.cshtml, creates one row in the table
    /// </summary>
    /// <param name="indexRI"></param>
    /// <returns></returns>
    public async Task<IActionResult> GetRecipeIngredientPartial(int indexRI)
    {
        var newIngredient = new RecipeIngredient();
        // the indexRI is tracking which row in the table is active
        ViewData["indexRI"] = indexRI;

        // Populate dropdown lists for the partial view.
        ViewBag.PantryIngredients = await _pantryIngredientService.GetPantryIngredientsSelectListAsync();
        ViewBag.Measures = await _measureService.GetMeasuresSelectListAsync();

        return PartialView("_RecipeIngredientPartial", newIngredient);
    }

    // GET: Edit Recipe
    public async Task<IActionResult> EditRecipe(int id)
    {
        var recipe = await _recipeRepository.GetQueryable()
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Measure)
            .FirstOrDefaultAsync(r => r.RecipeId == id);

        if (recipe == null) return NotFound();

        var recipeViewModel = new RecipeViewModel
        {
            Recipe = recipe,
            Measures = await _measureService.GetMeasuresSelectListAsync()
        };

        ViewBag.PantryIngredients = await _pantryIngredientService.GetPantryIngredientsSelectListAsync();
        ViewBag.Measures = await _measureService.GetMeasuresSelectListAsync();

        return View(recipeViewModel);
    }

    // POST: Update Recipe
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRecipe(RecipeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Collect all error messages
            var allErrors = ModelState
                .SelectMany(x => x.Value!.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            // Attach errors to the ViewBag so they can be displayed in the view
            ViewBag.ModelErrors = allErrors;

            return View(model);
        }

        var existingRecipe = await _recipeRepository.GetQueryable()
            .Include(er => er.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(er => er.RecipeIngredients)
                .ThenInclude(ri => ri.Measure)
            .FirstOrDefaultAsync(er => er.RecipeId == model.Recipe.RecipeId);

        if (existingRecipe == null) return NotFound();

        // map the updates to simple properties
        existingRecipe.RecipeName = model.Recipe.RecipeName;
        existingRecipe.Instructions = model.Recipe.Instructions;

        // Reconcile RecipeIngredients
        var updatedIngredients = model.Recipe.RecipeIngredients;
        var currentIngredients = existingRecipe.RecipeIngredients;

        // Remove ingredients that are missing in the updated list
        foreach (var existing in currentIngredients.ToList())
        {
            if (!updatedIngredients.Any(ui => ui.RecipeIngredientId == existing.RecipeIngredientId))
            {
                currentIngredients.Remove(existing);
                await _recipeIngredientRepository.DeleteAsync(existing.RecipeIngredientId);
            }
        }

        // Update or add new ingredients
        foreach (var updated in updatedIngredients)
        {
            if (updated.RecipeIngredientId > 0)
            {
                // this is an existing ingredient - just update the fields
                var existing = currentIngredients.FirstOrDefault(ci => ci.RecipeIngredientId == updated.RecipeIngredientId);
                if (existing != null)
                {
                    existing.IngredientId = updated.IngredientId;
                    existing.Quantity = updated.Quantity;
                    existing.MeasureId = updated.MeasureId;
                }
            }
            else
            {
                // this is a newly added ingredient.  Create a new RecipeIngredient entity
                RecipeIngredient newIngredient = new RecipeIngredient
                {
                    IngredientId = updated.IngredientId,
                    Quantity = updated.Quantity,
                    MeasureId = updated.MeasureId
                };
                currentIngredients.Add(newIngredient);
                await _recipeIngredientRepository.AddAsync(newIngredient);
            }
        }

        await _recipeRepository.UpdateAsync(existingRecipe);
        return RedirectToAction(nameof(RecipeDetails), new { id = model.Recipe.RecipeId });

    }

    // POST: Delete Recipe
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);

        if (recipe == null)
        {
            TempData["Error"] = "Recipe not found";
            return RedirectToAction("ViewRecipes");
        }

        try
        {
            await _recipeRepository.DeleteAsync(id);
            TempData["Message"] = "Recipe successfully deleted.";
        }
        catch (Exception)
        {
            TempData["Error"] = "Failed to delete Recipe.";
        }

        return RedirectToAction("ViewRecipes");
    }
    
    /// <summary>
    ///     Display a table listing all calculated Recipes by name, cost
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> ViewCalculatedRecipes()
    {
        var recipes = await _recipeRepository.GetAllAsync();
        var model = new List<RecipeViewModel>();

        foreach (var recipe in recipes)
        {
            var viewModel = new RecipeViewModel
            {
                Recipe = recipe,
                TotalCost = await _recipeCalculationService.CalculateRecipeCostAsync(recipe.RecipeId)
            };
            model.Add(viewModel);
        }

        return View(model);
    }

}