using UnitConverter.Api.Domain;
using UnitConverter.Api.Errors;
using UnitConverter.Api.Services;

namespace UnitConverter.Api.Tests;

public class ConversionServiceTests
{
    private readonly ConversionService _sut = new(BuiltInUnits.Create());

    [Theory]
    [InlineData(1, "meter", "foot", 3.280839895)]
    [InlineData(1, "mile", "meter", 1609.344)]
    [InlineData(12, "in", "ft", 1)]
    [InlineData(100, "C", "F", 212)]
    [InlineData(32, "F", "C", 0)]
    [InlineData(0, "celsius", "kelvin", 273.15)]
    [InlineData(-40, "C", "F", -40)]
    [InlineData(1, "kg", "lb", 2.2046226218)]
    [InlineData(16, "oz", "lb", 1)]
    [InlineData(1, "gal", "qt", 4)]
    public void Converts_known_values(double value, string from, string to, double expected)
    {
        var result = _sut.Convert(value, from, to);

        Assert.Equal(expected, result.Value, 6);
    }

    [Theory]
    [InlineData("METER", "Feet")]
    [InlineData(" m ", "ft")]
    [InlineData("metres", "FT")]
    public void Unit_lookup_is_case_insensitive_and_accepts_aliases(string from, string to)
    {
        var result = _sut.Convert(1, from, to);

        Assert.Equal("meter", result.From.Name);
        Assert.Equal("foot", result.To.Name);
    }

    [Fact]
    public void Round_trip_returns_original_value()
    {
        var there = _sut.Convert(123.456, "km", "mi");
        var back = _sut.Convert(there.Value, "mi", "km");

        Assert.Equal(123.456, back.Value, 6);
    }

    [Fact]
    public void Same_unit_returns_same_value() =>
        Assert.Equal(42, _sut.Convert(42, "kg", "kg").Value, 9);

    [Theory]
    [InlineData("parsec", "meter", ConversionError.UnknownUnit)]
    [InlineData("meter", "kg", ConversionError.IncompatibleUnits)]
    [InlineData("", "meter", ConversionError.UnknownUnit)]
    public void Rejects_invalid_units(string from, string to, ConversionError expected)
    {
        var ex = Assert.Throws<ConversionException>(() => _sut.Convert(1, from, to));

        Assert.Equal(expected, ex.Code);
    }

    [Fact]
    public void Rejects_unit_outside_requested_category()
    {
        var ex = Assert.Throws<ConversionException>(() => _sut.Convert(1, "meter", "foot", "mass"));

        Assert.Equal(ConversionError.UnitNotInCategory, ex.Code);
    }

    [Fact]
    public void Rejects_unknown_category()
    {
        var ex = Assert.Throws<ConversionException>(() => _sut.Convert(1, "meter", "foot", "colour"));

        Assert.Equal(ConversionError.UnknownCategory, ex.Code);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Rejects_non_finite_values(double value)
    {
        var ex = Assert.Throws<ConversionException>(() => _sut.Convert(value, "m", "ft"));

        Assert.Equal(ConversionError.InvalidValue, ex.Code);
    }

    [Theory]
    [InlineData(-300, "C", "K")]
    [InlineData(-1, "K", "C")]
    [InlineData(-500, "F", "C")]
    public void Rejects_temperatures_below_absolute_zero(double value, string from, string to)
    {
        var ex = Assert.Throws<ConversionException>(() => _sut.Convert(value, from, to));

        Assert.Equal(ConversionError.BelowPhysicalLimit, ex.Code);
    }

    [Fact]
    public void Absolute_zero_itself_is_allowed() =>
        Assert.Equal(0, _sut.Convert(-273.15, "C", "K").Value, 6);

    [Theory]
    [InlineData(1000, "INR", "USD", 12)]
    [InlineData(100, "USD", "EUR", 108)]
    [InlineData(100, "EUR", "GBP", 85.0393700787)]
    [InlineData(100, "USD", "CNY", 724.6376811594)]
    [InlineData(1, "KWD", "USD", 3.25)]
    public void Converts_currencies(double value, string from, string to, double expected)
    {
        var result = _sut.Convert(value, from, to, BuiltInUnits.Currency);

        Assert.Equal(expected, result.Value, 6);
    }

    [Fact]
    public void Rejects_currency_to_physical_unit_conversion()
    {
        var ex = Assert.Throws<ConversionException>(
            () => _sut.Convert(100, "INR", "kg"));

        Assert.Equal(ConversionError.IncompatibleUnits, ex.Code);
    }
}
