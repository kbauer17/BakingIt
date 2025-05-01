using BakingIt.Models;
using Microsoft.AspNetCore.Mvc;

public class IngredientController : Controller
{
    private readonly IBakingItRepository<Conversion> _conversionRepository;
    private readonly IBakingItRepository<Ingredient> _ingredientRepository;

    public IngredientController(
        IBakingItRepository<Conversion> conversionRepository,
        IBakingItRepository<Ingredient> ingredientRepository)
    {
        _conversionRepository = conversionRepository;
        _ingredientRepository = ingredientRepository;
    }

    // GET: Display all conversions and ingredients
    public async Task<IActionResult> Index()
    {
        var conversions = await _conversionRepository.GetAllAsync();
        var ingredients = await _ingredientRepository.GetAllAsync();

        var viewModel = new ConversionIngredientViewModel
        {
            Conversions = conversions.ToList(),
            Ingredients = ingredients.ToList()
        };

        return View(viewModel);
    }

    #region Ingredient
    public async Task<IActionResult> ViewIngredients()
    {
        var ingredients = await _ingredientRepository.GetAllAsync();
        return View(ingredients);
    }

    /// <summary>
    ///     Create a new Ingredient - Getter
    /// </summary>
    /// <returns></returns>
    public IActionResult CreateIngredient()
    {
        return View("CreateIngredient");
    }

    /// <summary>
    ///     Create a new Ingredient - Setter
    /// </summary>
    /// <param name="ingredient"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateIngredient(Ingredient ingredient)
    {
        if (ModelState.IsValid)
        {
            await _ingredientRepository.AddAsync(ingredient);
            return RedirectToAction(nameof(ViewIngredients));
        }
        else
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }
            return View(ingredient);
        }
    }

    /// <summary>
    ///     Edit an Ingredient - Getter
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> EditIngredient(int id)
    {
        var ingredient = await _ingredientRepository.GetByIdAsync(id);
        return View(ingredient);
    }

    /// <summary>
    ///     Edit an Ingredient - Setter
    /// </summary>
    /// <param name="ingredient"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditIngredient(Ingredient ingredient)
    {
        if(ModelState.IsValid)
        {
            await _ingredientRepository.UpdateAsync(ingredient);
            return RedirectToAction(nameof(ViewIngredients));
        }
        else
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }
            return View(ingredient);
        }
    }

    public async Task<IActionResult> DeleteIngredient(int id)
    {
        var ingredient = await _ingredientRepository.GetByIdAsync(id);
        if(ingredient == null)
        {
            TempData["Error"] = "Ingredient not found.";
            return RedirectToAction("ViewIngredients"); 
        }
        try
        {
            await _ingredientRepository.DeleteAsync(id);
            TempData["Message"] = "Ingredient successfully deleted.";
        }
        catch (Exception)
        {
            TempData["Error"] = "Failed to delete Ingredient, most likely because it is used in a Recipe.  Remove it from the recipe and try again.";
        }
        return RedirectToAction("ViewIngredients");
    }
    #endregion  // end of Ingredient section

    #region Conversion
    //  adding a Conversion
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddConversion(Conversion conversion)
    {
        if (ModelState.IsValid)
        {
            await _conversionRepository.AddAsync(conversion);
            return RedirectToAction(nameof(Index));
        }
        return View(conversion);
    }
    #endregion  // end of Conversion section

}