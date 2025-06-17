using BakingIt.Models;
using BakingIt.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class ConversionController : Controller
{
    private readonly IBakingItRepository<Conversion> _conversionRepository;
    private readonly IMeasureService _measureService;
    private readonly IPantryIngredientService _pantryIngredientService;

    public ConversionController(
        IBakingItRepository<Conversion> conversionRepository,
        IMeasureService measureService, IPantryIngredientService pantryIngredientService)
    {
        _conversionRepository = conversionRepository;
        _measureService = measureService;
        _pantryIngredientService = pantryIngredientService;
    }

    /// <summary>
    ///     Display a list of all Conversions - Getter
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> ViewConversions()
    {
        var conversions = await _conversionRepository.GetAllAsync();
        return View(conversions);
    }

    /// <summary>
    ///     Create a new Conversion - Getter
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> CreateConversion()
    {
        //Populate dropdown lists for Measure, Ingredients
        ViewBag.Measures = await _measureService.GetMeasuresSelectListAsync();
        ViewBag.PantryIngredients = await _pantryIngredientService.GetPantryIngredientsSelectListAsync();

        return View();
    }

    /// <summary>
    ///     Create a new Conversion - Setter
    /// </summary>
    /// <param name="conversion"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateConversion(Conversion conversion)
    {
        if (!ModelState.IsValid)
        {
            // Repopulate the dropdown lists if returning the view
            ViewBag.Measures = await _measureService.GetMeasuresSelectListAsync();
            ViewBag.PantryIngredients = await _pantryIngredientService.GetPantryIngredientsSelectListAsync();

            // Collect all error messages
            var allErrors = ModelState
                .SelectMany(x => x.Value!.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            // Attach errors to the ViewBag so they can be displayed in the view
            ViewBag.ModelErrors = allErrors;

            return View(conversion);
        }

        await _conversionRepository.AddAsync(conversion);
        return RedirectToAction(nameof(ViewConversions));
    }

    /// <summary>
    ///     Edit a Conversion - Getter
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> EditConversion(int id)
    {
        ViewBag.Measures = await _measureService.GetMeasuresSelectListAsync();
        ViewBag.PantryIngredients = await _pantryIngredientService.GetPantryIngredientsSelectListAsync();

        var conversion = await _conversionRepository.GetByIdAsync(id);
        return View(conversion);
    }

    /// <summary>
    ///     Edit a Conversion - Setter
    /// </summary>
    /// <param name="conversion"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditConversion(Conversion conversion)
    {
        if (!ModelState.IsValid)
        {
            // Repopulate the dropdown lists if returning the view
            ViewBag.Measures = await _measureService.GetMeasuresSelectListAsync();
            ViewBag.PantryIngredients = await _pantryIngredientService.GetPantryIngredientsSelectListAsync();

            // Collect all error messages
            var allErrors = ModelState
                .SelectMany(x => x.Value!.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            // Attach errors to the ViewBag so they can be displayed in the view
            ViewBag.ModelErrors = allErrors;

            return View(conversion);
        }

        await _conversionRepository.UpdateAsync(conversion);
        return RedirectToAction(nameof(ViewConversions));
    }

    public async Task<IActionResult> DeleteConversion(int id)
    {
        var conversion = await _conversionRepository.GetByIdAsync(id);
        if (conversion == null)
        {
            TempData["Error"] = "Conversion not found.";
            return RedirectToAction("ViewConversions");
        }
        try
        {
            await _conversionRepository.DeleteAsync(id);
            TempData["Message"] = "Conversion successfully deleted.";
        }
        catch (Exception)
        {
            TempData["Error"] = "Failed to delete Conversion, most likely because it is in use.";
        }
        return RedirectToAction("ViewConversions");
    }

}