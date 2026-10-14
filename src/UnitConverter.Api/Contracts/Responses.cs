using UnitConverter.Api.Domain;
using UnitConverter.Api.Services;

namespace UnitConverter.Api.Contracts;

public sealed record ConvertResponse(
    string Category,
    double InputValue,
    string FromUnit,
    string FromSymbol,
    string ToUnit,
    string ToSymbol,
    double Result)
{
    public static ConvertResponse From(ConversionResult r) =>
        new(r.From.Category, r.InputValue, r.From.Name, r.From.Symbol, r.To.Name, r.To.Symbol, r.Value);
}

public sealed record UnitDto(string Category, string Name, string Symbol, IReadOnlyList<string> Aliases)
{
    public static UnitDto From(UnitDefinition u) => new(u.Category, u.Name, u.Symbol, u.Aliases);
}
