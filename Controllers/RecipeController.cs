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

    // Inject repository via constructor
    public RecipeController(IBakingItRepository<Recipe> recipeRepository, IBakingItRepository<Ingredient> ingredientRepository, IBakingItRepository<RecipeIngredient> recipeIngredientRepository, IMeasureService measureService, IPantryIngredientService pantryIngredientService)
    {
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
        _recipeIngredientRepository = recipeIngredientRepository;
        _measureService = measureService;
        _pantryIngredientService = pantryIngredientService;
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
        var model = new RecipeViewModel();

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

        // Replace each model-bound Ingredient with is tracked instance from the database.
        // This is not affecting the RecipeIngredients
        foreach (var recipeIngredient in model.Recipe.RecipeIngredients)
        {
            var existingIngredient = await _ingredientRepository.GetByIdAsync(recipeIngredient.Ingredient.IngredientId);
            if (existingIngredient != null)
            {
                recipeIngredient.Ingredient= existingIngredient;
            }
        }

        await _recipeRepository.UpdateAsync(model.Recipe);
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

}