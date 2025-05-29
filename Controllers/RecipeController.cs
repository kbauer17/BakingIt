using BakingIt.Models;
using Microsoft.AspNetCore.Mvc;

public class RecipeController : Controller
{
    private readonly IBakingItRepository<Recipe> _recipeRepository;
    private readonly IBakingItRepository<Ingredient> _ingredientRepository;
    private readonly IBakingItRepository<RecipeIngredient> _recipeIngredientRepository;

    // Inject repository via constructor
    public RecipeController(IBakingItRepository<Recipe> recipeRepository, IBakingItRepository<Ingredient> ingredientRepository, IBakingItRepository<RecipeIngredient> recipeIngredientRepository)
    {
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
        _recipeIngredientRepository = recipeIngredientRepository;
    }

    // GET: Display all recipes
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

    // GET: Create Recipe Form
    public async Task<IActionResult> CreateRecipe()
    {
        ViewBag.Ingredients =await _ingredientRepository.GetAllAsync(); // Populate dropdown
        return View();
    }

    // POST: Create New Recipe
    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> CreateRecipe(Recipe recipe)
    // {
    //     if (ModelState.IsValid)
    //     {
    //         await _recipeRepository.AddAsync(recipe);
    //         return RedirectToAction(nameof(Index));
    //     }
    //     return View(recipe);
    // }

    // POST: Recipe/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRecipe(Recipe recipe, List<int> ingredientIds, List<decimal> quantities, List<string> units)
    {
        if (ModelState.IsValid)
        {
            await _recipeRepository.AddAsync(recipe);

            // Add ingredients to RecipeIngredient table
            for (int i = 0; i < ingredientIds.Count; i++)
            {
                var recipeIngredient = new RecipeIngredient
                {
                    RecipeId = recipe.RecipeId,
                    IngredientId = ingredientIds[i],
                    Quantity = quantities[i],
                    Unit = units[i]
                };

                await _recipeIngredientRepository.AddAsync(recipeIngredient);
            }
            return RedirectToAction(nameof(ViewRecipes)); 
        }
        ViewBag.Ingredients = await _ingredientRepository.GetAllAsync();
        return View(recipe);
    }

    // GET: Edit Recipe
    public async Task<IActionResult> Edit(int id)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null) return NotFound();

        return View(recipe);
    }

    // POST: Update Recipe
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Recipe recipe)
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
    public async Task<IActionResult> Delete(int id)
    {
        await _recipeRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}