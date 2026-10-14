using UnitConverter.Api.Domain;

namespace UnitConverter.Api.Services;

public sealed record ConversionResult(
    UnitDefinition From,
    UnitDefinition To,
    double InputValue,
    double Value);

public interface IConversionService
{
    /// <exception cref="Errors.ConversionException">Input is invalid or units are incompatible.</exception>
    ConversionResult Convert(double value, string fromUnit, string toUnit, string? category = null);
}
