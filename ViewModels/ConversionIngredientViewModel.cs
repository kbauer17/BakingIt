using BakingIt.Models;

public class ConversionIngredientViewModel
{
    public List<Conversion> Conversions { get; set; } = new();
    public List<Ingredient> Ingredients { get; set; } = new();

    // Default constructor (no arguments)
    public ConversionIngredientViewModel()
    {
        Conversions = new List<Conversion>();
        Ingredients = new List<Ingredient>();
    }

    // Parameterized constructor to initialize with data
    public ConversionIngredientViewModel(List<Conversion> conversions, List<Ingredient> ingredients)
    {
        Conversions = conversions ?? new List<Conversion>();
        Ingredients = ingredients ?? new List<Ingredient>();
    }

    public ConversionIngredientViewModel(List<Conversion> conversions)
    {
        Conversions = conversions ?? new List<Conversion>();
    }

    public ConversionIngredientViewModel(List<Ingredient> ingredients)
    {
        Ingredients = ingredients ?? new List<Ingredient>();
    }
}