using UnitConverter.Api.Domain;

namespace UnitConverter.Api.Tests;

public class UnitCatalogTests
{
    [Fact]
    public void Built_in_catalog_is_valid_and_covers_required_categories()
    {
        var catalog = BuiltInUnits.Create();

        foreach (var name in new[] { "length", "temperature", "mass" })
        {
            Assert.True(catalog.TryGetCategory(name, out _), $"Missing category {name}");
            Assert.True(catalog.Units.Count(u => u.Category == name) >= 2);
        }
    }

    [Fact]
    public void Duplicate_identifiers_fail_fast()
    {
        var categories = new[] { new UnitCategory("length") };
        var units = new[]
        {
            new UnitDefinition("length", "meter", "m", 1),
            new UnitDefinition("length", "minute-of-arc", "M", 2), // "M" collides with "m" (case-insensitive)
        };

        Assert.Throws<InvalidOperationException>(() => new InMemoryUnitCatalog(categories, units));
    }

    [Fact]
    public void Unit_with_unknown_category_fails_fast()
    {
        var units = new[] { new UnitDefinition("ghost", "boo", "b", 1) };

        Assert.Throws<InvalidOperationException>(
            () => new InMemoryUnitCatalog(Array.Empty<UnitCategory>(), units));
    }

    [Fact]
    public void Zero_factor_is_rejected() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new UnitDefinition("length", "x", "x", 0));
}
