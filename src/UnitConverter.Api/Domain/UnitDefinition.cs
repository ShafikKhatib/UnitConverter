namespace UnitConverter.Api.Domain;

/// <summary>
/// Describes a unit as an affine transformation to its category's base unit:
/// <c>baseValue = value * ToBaseFactor + ToBaseOffset</c>.
/// This single formula covers pure scaling (length, mass, volume) and
/// offset conversions (temperature), so every conversion is
/// "unit A -> base -> unit B" and N units need N definitions, not N*N rules.
/// </summary>
public sealed class UnitDefinition
{
    public UnitDefinition(
        string category,
        string name,
        string symbol,
        double toBaseFactor,
        double toBaseOffset = 0,
        IEnumerable<string>? aliases = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);

        if (!double.IsFinite(toBaseFactor) || toBaseFactor == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(toBaseFactor), "Factor must be finite and non-zero.");
        }

        Category = category;
        Name = name;
        Symbol = symbol;
        ToBaseFactor = toBaseFactor;
        ToBaseOffset = toBaseOffset;
        Aliases = (aliases ?? Array.Empty<string>()).ToArray();
    }

    public string Category { get; }
    public string Name { get; }
    public string Symbol { get; }
    public double ToBaseFactor { get; }
    public double ToBaseOffset { get; }
    public IReadOnlyList<string> Aliases { get; }

    /// <summary>All identifiers (case-insensitive) that resolve to this unit.</summary>
    public IEnumerable<string> Keys =>
        new[] { Name, Symbol }.Concat(Aliases).Distinct(StringComparer.OrdinalIgnoreCase);
}
