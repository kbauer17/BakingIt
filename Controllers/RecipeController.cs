using BakingIt.Models;
using BakingIt.Services;
using BakingIt.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Details(int id)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null) return NotFound();

        return View(recipe);
    }

    /// <summary>
    ///     Create a new Recipe - Getter
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult CreateRecipe()
    {
        var viewModel = new RecipeViewModel();
        return View(viewModel);
    }

    /// <summary>
    ///     Create a new Recipe - setter
    /// </summary>
    /// <param name="viewModel"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRecipe(RecipeViewModel viewModel)
    {
        // Re-populate the lists, otherwise the form will lose this on a failed submission.
        viewModel.Measures = await _measureService.GetMeasuresSelectListAsync();
        ViewBag.PantryIngredients = await _pantryIngredientService.GetPantryIngredientsSelectListAsync();

        // Here you might check for ModelState validity. 
        // Note: You could also validate the inner Recipe entity if you have validations on it.
        if (!ModelState.IsValid)
        {
            // Collect all error messages
            var allErrors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            // Attach errors to the ViewBag so they can be displayed in the view
            ViewBag.ModelErrors = allErrors;

            return View(viewModel);
        }

        // In this approach the view model already contains the Recipe entity. 
        // Save it directly with your repository.
        await _recipeRepository.AddAsync(viewModel.Recipe);

        return RedirectToAction("ViewRecipes");
    }

    // GET: Edit Recipe
    public async Task<IActionResult> EditRecipe(int id)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null) return NotFound();

        return View(recipe);
    }

    // POST: Update Recipe
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRecipe(int id, Recipe recipe)
    {
        if (id != recipe.RecipeId) return BadRequest();

        if (ModelState.IsValid)
        {
            await _recipeRepository.UpdateAsync(recipe);
            return RedirectToAction(nameof(Index));
        }
        return View(recipe);
    }

    // POST: Delete Recipe
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        await _recipeRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
    
    /// <summary>
    ///     Getter for the _RecipeIngredientPartial.cshtml, called via AJAX within CreateRecipe.cshtml
    /// </summary>
    /// <param name="indexRi"></param>
    /// <returns></returns>
    public async Task<IActionResult> GetRecipeIngredientPartial(int indexRi)
    {
        var newIngredient = new RecipeIngredient();
        ViewData["indexRI"] = indexRi;
        ViewBag.PantryIngredients = await _pantryIngredientService.GetPantryIngredientsSelectListAsync();
        ViewBag.Measures = await _measureService.GetMeasuresSelectListAsync();
        return PartialView("_RecipeIngredientPartial", newIngredient);
    }
}