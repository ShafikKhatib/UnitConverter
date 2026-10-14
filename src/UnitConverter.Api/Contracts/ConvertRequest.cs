using System.ComponentModel.DataAnnotations;

namespace UnitConverter.Api.Contracts;

public sealed class ConvertRequest
{
    /// <summary>The number to convert.</summary>
    [Required]
    public double? Value { get; init; }

    /// <summary>Source unit: name, symbol or alias (e.g. "meter", "m", "metres").</summary>
    [Required]
    public string? FromUnit { get; init; }

    /// <summary>Target unit: name, symbol or alias.</summary>
    [Required]
    public string? ToUnit { get; init; }

    /// <summary>Optional. When set, both units must belong to this category.</summary>
    public string? Category { get; init; }
}
