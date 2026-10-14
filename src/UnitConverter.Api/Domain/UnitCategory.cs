namespace UnitConverter.Api.Domain;

/// <summary>
/// A family of units that can be converted between each other (e.g. length).
/// </summary>
/// <param name="Name">Lower-case category name used in the API.</param>
/// <param name="MinimumBaseValue">
/// Optional physical lower bound, expressed in the category's base unit
/// (e.g. -273.15 °C for temperature). Null means unbounded.
/// </param>
public sealed record UnitCategory(string Name, double? MinimumBaseValue = null);
