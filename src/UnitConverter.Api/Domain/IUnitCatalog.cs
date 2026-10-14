namespace UnitConverter.Api.Domain;

/// <summary>
/// Source of truth for categories and units. Today it is in-memory and hard-coded;
/// the interface lets a database or configuration file replace it without
/// touching the conversion logic or the API layer.
/// </summary>
public interface IUnitCatalog
{
    IReadOnlyCollection<UnitCategory> Categories { get; }

    IReadOnlyCollection<UnitDefinition> Units { get; }

    bool TryGetCategory(string name, out UnitCategory category);

    bool TryFindUnit(string identifier, out UnitDefinition unit);
}
