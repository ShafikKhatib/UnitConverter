namespace UnitConverter.Api.Errors;

public enum ConversionError
{
    InvalidValue,
    UnknownUnit,
    UnknownCategory,
    UnitNotInCategory,
    IncompatibleUnits,
    BelowPhysicalLimit,
    ResultOutOfRange,
}

/// <summary>A predictable, caller-correctable failure (maps to HTTP 400).</summary>
public sealed class ConversionException(ConversionError code, string message) : Exception(message)
{
    public ConversionError Code { get; } = code;
}
