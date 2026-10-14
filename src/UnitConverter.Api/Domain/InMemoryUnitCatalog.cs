namespace UnitConverter.Api.Domain;

/// <summary>
/// Dictionary-backed catalog. Validates at construction that every unit
/// belongs to a known category and that no identifier is ambiguous, so
/// data mistakes fail fast at start-up instead of producing wrong answers.
/// </summary>
public sealed class InMemoryUnitCatalog : IUnitCatalog
{
    private readonly Dictionary<string, UnitCategory> _categories;
    private readonly Dictionary<string, UnitDefinition> _lookup = new(StringComparer.OrdinalIgnoreCase);

    public InMemoryUnitCatalog(IEnumerable<UnitCategory> categories, IEnumerable<UnitDefinition> units)
    {
        _categories = categories.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
        Units = units.ToList();

        foreach (var unit in Units)
        {
            if (!_categories.ContainsKey(unit.Category))
            {
                throw new InvalidOperationException(
                    $"Unit '{unit.Name}' references unknown category '{unit.Category}'.");
            }

            foreach (var key in unit.Keys)
            {
                if (!_lookup.TryAdd(key, unit))
                {
                    throw new InvalidOperationException($"Duplicate unit identifier '{key}'.");
                }
            }
        }
    }

    public IReadOnlyCollection<UnitCategory> Categories => _categories.Values;

    public IReadOnlyCollection<UnitDefinition> Units { get; }

    public bool TryGetCategory(string name, out UnitCategory category) =>
        _categories.TryGetValue(name.Trim(), out category!);

    public bool TryFindUnit(string identifier, out UnitDefinition unit) =>
        _lookup.TryGetValue(identifier.Trim(), out unit!);
}
