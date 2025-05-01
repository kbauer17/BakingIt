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

    // adding an Ingredient
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddIngredient(Ingredient ingredient)
    {
        if (ModelState.IsValid)
        {
            await _ingredientRepository.AddAsync(ingredient);
            return RedirectToAction(nameof(Index));
        }
        return View(ingredient);
    }
}