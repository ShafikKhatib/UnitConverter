using UnitConverter.Api.Domain;
using UnitConverter.Api.Errors;

namespace UnitConverter.Api.Services;

public sealed class ConversionService(IUnitCatalog catalog) : IConversionService
{
    // Rounds away binary floating-point noise (e.g. 212.00000000000003 -> 212).
    private const int ResultDecimals = 10;

    public ConversionResult Convert(double value, string fromUnit, string toUnit, string? category = null)
    {
        if (!double.IsFinite(value))
        {
            throw new ConversionException(ConversionError.InvalidValue, "Value must be a finite number.");
        }

        var from = Resolve(fromUnit);
        var to = Resolve(toUnit);

        if (!string.IsNullOrWhiteSpace(category))
        {
            if (!catalog.TryGetCategory(category, out var requested))
            {
                throw new ConversionException(ConversionError.UnknownCategory, $"Unknown category '{category}'.");
            }

            foreach (var unit in new[] { from, to })
            {
                if (!string.Equals(unit.Category, requested.Name, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ConversionException(
                        ConversionError.UnitNotInCategory,
                        $"Unit '{unit.Name}' is a {unit.Category} unit, not a {requested.Name} unit.");
                }
            }
        }

        if (!string.Equals(from.Category, to.Category, StringComparison.OrdinalIgnoreCase))
        {
            throw new ConversionException(
                ConversionError.IncompatibleUnits,
                $"Cannot convert {from.Category} ('{from.Name}') to {to.Category} ('{to.Name}').");
        }

        // unit A -> base unit -> unit B
        var baseValue = (value * from.ToBaseFactor) + from.ToBaseOffset;

        catalog.TryGetCategory(from.Category, out var cat);
        if (cat.MinimumBaseValue is { } minimum && baseValue < minimum - 1e-9)
        {
            throw new ConversionException(
                ConversionError.BelowPhysicalLimit,
                $"{value} {from.Name} is below the physical minimum for {from.Category} (absolute zero).");
        }

        var result = (baseValue - to.ToBaseOffset) / to.ToBaseFactor;

        if (!double.IsFinite(result))
        {
            throw new ConversionException(ConversionError.ResultOutOfRange, "Result is out of range.");
        }

        return new ConversionResult(from, to, value, Math.Round(result, ResultDecimals));
    }

    private UnitDefinition Resolve(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier) || !catalog.TryFindUnit(identifier, out var unit))
        {
            throw new ConversionException(ConversionError.UnknownUnit, $"Unknown unit '{identifier}'.");
        }

        return unit;
    }
}
