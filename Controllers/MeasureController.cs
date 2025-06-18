using BakingIt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class MeasureController : Controller
{
    private readonly IBakingItRepository<Measure> _measureRepository;

    public MeasureController(
        IBakingItRepository<Measure> measureRepository)
    {
        _measureRepository = measureRepository;
    }

    /// <summary>
    ///     View a table of all Measures - Getter
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> ViewMeasures()
    {
        var measures = await _measureRepository.GetAllAsync();
        return View(measures);
    }

    /// <summary>
    ///     Create a new Measure - Getter
    /// </summary>
    /// <returns></returns>
    public IActionResult CreateMeasure()
    {
        return View("CreateMeasure");
    }

    /// <summary>
    ///     Create a new Measure - Setter
    /// </summary>
    /// <param name="measure"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMeasure(Measure measure)
    {
        // check for duplicate MeasureName
        if (await MeasureExistsAsync(measure.MeasureName))
        {
            ModelState.AddModelError("MeasureName", "This ingredient already exists.");
            return View(measure);
        }

        if (ModelState.IsValid)
        {
            await _measureRepository.AddAsync(measure);
            return RedirectToAction(nameof(ViewMeasures));
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
            return View(measure);
        }
    }

    /// <summary>
    ///     Edit existing Measure - Getter
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> EditMeasure(int id)
    {
        var measure = await _measureRepository.GetByIdAsync(id);
        return View(measure);
    }

    /// <summary>
    ///     Edit an existing Measure - Setter
    /// </summary>
    /// <param name="measure"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditMeasure(Measure measure)
    {
        if (ModelState.IsValid)
        {
            await _measureRepository.UpdateAsync(measure);
            return RedirectToAction(nameof(ViewMeasures));
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
            return View(measure);
        }
    }

    /// <summary>
    ///     Delete an existing Measure - Setter
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> DeleteMeasure(int id)
    {
        var measure = await _measureRepository.GetByIdAsync(id);
        if (measure == null)
        {
            TempData["Error"] = "Measure not found.";
            return RedirectToAction("ViewMeasures");
        }
        try
        {
            await _measureRepository.DeleteAsync(id);
            TempData["Message"] = "Measure successfully deleted.";
        }
        catch (Exception)
        {
            TempData["Error"] = "Failed to delete Measure, most likely because it is in use.";
        }
        return RedirectToAction("ViewMeasures");
    }

    /// <summary>
    ///     Boolean check for duplicate name - called by CreateMeasure setter
    /// </summary>
    /// <param name="measureName"></param>
    /// <returns></returns>
    private async Task<bool> MeasureExistsAsync(string measureName)
    {
        // Normalize the name for comparison (remove spaces & make lowercase)
        string normalizedName = new string(measureName
            .Where(c => !char.IsWhiteSpace(c))
            .ToArray())
            .ToLower();

        return await _measureRepository.ExistsAsync(i =>
            i.MeasureName.Replace(" ", "").ToLower() == normalizedName);
    }
}