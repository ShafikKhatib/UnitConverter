namespace UnitConverter.Api.Domain;

/// <summary>
/// Hard-coded unit data for this version of the service.
/// To add a unit, add one line here (base unit factor only) - nothing else changes.
/// Base units: length = meter, mass = kilogram, temperature = celsius, volume = liter.
/// </summary>
public static class BuiltInUnits
{
    public const string Length = "length";
    public const string Mass = "mass";
    public const string Temperature = "temperature";
    public const string Volume = "volume";

    private const double AbsoluteZeroCelsius = -273.15;

    public static IUnitCatalog Create() => new InMemoryUnitCatalog(Categories, Units);

    private static readonly UnitCategory[] Categories =
    [
        new(Length),
        new(Mass),
        new(Temperature, MinimumBaseValue: AbsoluteZeroCelsius),
        new(Volume),
    ];

    private static readonly UnitDefinition[] Units =
    [
        // Length (base: meter)
        Scale(Length, "millimeter", "mm", 0.001, "millimeters", "millimetre", "millimetres"),
        Scale(Length, "centimeter", "cm", 0.01, "centimeters", "centimetre", "centimetres"),
        Scale(Length, "meter", "m", 1, "meters", "metre", "metres"),
        Scale(Length, "kilometer", "km", 1000, "kilometers", "kilometre", "kilometres"),
        Scale(Length, "inch", "in", 0.0254, "inches"),
        Scale(Length, "foot", "ft", 0.3048, "feet"),
        Scale(Length, "yard", "yd", 0.9144, "yards"),
        Scale(Length, "mile", "mi", 1609.344, "miles"),

        // Mass (base: kilogram)
        Scale(Mass, "milligram", "mg", 0.000001, "milligrams"),
        Scale(Mass, "gram", "g", 0.001, "grams"),
        Scale(Mass, "kilogram", "kg", 1, "kilograms", "kilo", "kilos"),
        Scale(Mass, "tonne", "t", 1000, "tonnes", "metric ton", "metric tons"),
        Scale(Mass, "ounce", "oz", 0.028349523125, "ounces"),
        Scale(Mass, "pound", "lb", 0.45359237, "pounds", "lbs"),

        // Temperature (base: celsius). baseValue = value * factor + offset
        new(Temperature, "celsius", "°C", 1, 0, new[] { "c", "degree celsius", "degrees celsius" }),
        new(Temperature, "fahrenheit", "°F", 5.0 / 9.0, -160.0 / 9.0, new[] { "f", "degree fahrenheit", "degrees fahrenheit" }),
        new(Temperature, "kelvin", "K", 1, -273.15, new[] { "kelvins" }),

        // Volume (base: liter, US customary)
        Scale(Volume, "milliliter", "ml", 0.001, "milliliters", "millilitre", "millilitres"),
        Scale(Volume, "liter", "l", 1, "liters", "litre", "litres"),
        Scale(Volume, "fluid ounce", "fl oz", 0.0295735295625, "fluid ounces", "us fluid ounce"),
        Scale(Volume, "cup", "cup", 0.2365882365, "cups"),
        Scale(Volume, "pint", "pt", 0.473176473, "pints"),
        Scale(Volume, "quart", "qt", 0.946352946, "quarts"),
        Scale(Volume, "gallon", "gal", 3.785411784, "gallons", "us gallon"),
    ];

    private static UnitDefinition Scale(string category, string name, string symbol, double factor, params string[] aliases) =>
        new(category, name, symbol, factor, 0, aliases);
}
