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
        // check for duplicate IngredientName
        if (await IngredientExistsAsync(ingredient.IngredientName))
        {
            ModelState.AddModelError("IngredientName", "This ingredient already exists.");
            ViewBag.Measures = await GetMeasuresSelectListAsync();
            return View(ingredient);
        }

        // Lookup Measure from database using MeasureId
        var measure = await _measureRepository.GetByIdAsync(ingredient.MeasureId);

        if (measure == null)
        {
            ModelState.AddModelError("MeasureId", "Invalid Measure selected.");
            ViewBag.Measures = await GetMeasuresSelectListAsync();
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
                // note:  the <span asp-validation-for="...> will display these error messages in the view
            }
            ViewBag.Measures = await GetMeasuresSelectListAsync();
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
            ViewBag.Measures = await GetMeasuresSelectListAsync();
            return View(ingredient);
        }

        // Assign Measure navigation property before model validation
        ingredient.Measure = measure;

        if (ModelState.IsValid)
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

    /// <summary>
    ///     Delete an existing Ingredient - Setter
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> DeleteIngredient(int id)
    {
        var ingredient = await _ingredientRepository.GetByIdAsync(id);
        if (ingredient == null)
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

    #region Private Methods
    private async Task<SelectList> GetMeasuresSelectListAsync()
    {
        var measures = await _measureRepository.GetAllAsync();

        // Order measures alphabetically by MeasureName
        var sortedMeasures = measures.OrderBy(m => m.MeasureName);

        return new SelectList(sortedMeasures, "MeasureId", "MeasureName");
    }
    
    /// <summary>
    ///     Check for existing Ingredient Name - called by CreateIngredient
    /// </summary>
    /// <param name="ingredientName"></param>
    /// <returns></returns>
    private async Task<bool> IngredientExistsAsync(string ingredientName)
    {
        // Normalize the name for comparison (remove spaces & make lowercase)
        string normalizedName = new string(ingredientName
            .Where(c => !char.IsWhiteSpace(c))
            .ToArray())
            .ToLower();

        return await _ingredientRepository.ExistsAsync(i =>
            i.IngredientName.Replace(" ", "").ToLower() == normalizedName);
    }

    #endregion
}