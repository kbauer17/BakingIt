using BakingIt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class IngredientController : Controller
{
    private readonly IBakingItRepository<Conversion> _conversionRepository;
    private readonly IBakingItRepository<Ingredient> _ingredientRepository;
    private readonly IBakingItRepository<Measure> _measureRepository;

    public IngredientController(
        IBakingItRepository<Conversion> conversionRepository,
        IBakingItRepository<Ingredient> ingredientRepository,
        IBakingItRepository<Measure> measureRepository)
    {
        _conversionRepository = conversionRepository;
        _ingredientRepository = ingredientRepository;
        _measureRepository = measureRepository;
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
        var ingredients = await _ingredientRepository
            .GetAllAsync(i => i.Include(x => x.Measure));
        return View(ingredients);
    }

    /// <summary>
    ///     Create a new Ingredient - Getter
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> CreateIngredient()
    {
        ViewBag.Measures = await GetMeasuresSelectListAsync();
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
        // Lookup Measure from database using MeasureId
        var measure = await _measureRepository.GetByIdAsync(ingredient.MeasureId);

        if (measure == null)
        {
            ModelState.AddModelError("MeasureId", "Invalid Measure selected.");
            return View(ingredient);
        }

        // Assign Measure navigation property before model validation
        ingredient.Measure = measure;

        if (ModelState.IsValid)
        {
            await _ingredientRepository.AddAsync(ingredient);
            return RedirectToAction(nameof(ViewIngredients));
        }
        else
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("\n" + error.ErrorMessage + "\n");
                Console.ResetColor();
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
        ViewBag.Measures = await GetMeasuresSelectListAsync();
        
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
        // Lookup Measure from database using MeasureId
        var measure = await _measureRepository.GetByIdAsync(ingredient.MeasureId);

        if (measure == null)
        {
            ModelState.AddModelError("MeasureId", "Invalid Measure selected.");
            return View(ingredient);
        }

        // Assign Measure navigation property before model validation
        ingredient.Measure = measure;

        if(ModelState.IsValid)
        {
            await _ingredientRepository.UpdateAsync(ingredient);
            return RedirectToAction(nameof(ViewIngredients));
        }
        else
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("\n" + error.ErrorMessage + "\n");
                Console.ResetColor();
            }

            // Repopulate ViewBag.Measures before returning to the view
            ViewBag.Measures = await GetMeasuresSelectListAsync();
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

    #region Private Methods
    private async Task<SelectList> GetMeasuresSelectListAsync()
    {
        var measures = await _measureRepository.GetAllAsync();
        
        // Order measures alphabetically by MeasureName
        var sortedMeasures = measures.OrderBy(m => m.MeasureName);

        return new SelectList(sortedMeasures, "MeasureId", "MeasureName");
    }
    #endregion
}