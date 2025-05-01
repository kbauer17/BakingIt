using BakingIt.Models;
using Microsoft.AspNetCore.Mvc;

public class RecipeController : Controller
{
    private readonly IBakingItRepository<Recipe> _recipeRepository;

    // Inject repository via constructor
    public RecipeController(IBakingItRepository<Recipe> recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    // GET: Display all recipes
    public async Task<IActionResult> Index()
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
    public IActionResult Create()
    {
        return View();
    }

    // POST: Create New Recipe
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Recipe recipe)
    {
        if (ModelState.IsValid)
        {
            await _recipeRepository.AddAsync(recipe);
            return RedirectToAction(nameof(Index));
        }
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