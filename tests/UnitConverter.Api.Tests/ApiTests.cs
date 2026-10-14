using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace UnitConverter.Api.Tests;

public class ApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Post_converts_value()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/conversions",
            new { value = 100, fromUnit = "C", toUnit = "F" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(212, json.RootElement.GetProperty("result").GetDouble(), 6);
        Assert.Equal("temperature", json.RootElement.GetProperty("category").GetString());
    }

    [Fact]
    public async Task Get_converts_value()
    {
        var response = await _client.GetAsync("/api/v1/conversions?value=1&from=kg&to=lb");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(2.2046226218, json.RootElement.GetProperty("result").GetDouble(), 6);
    }

    [Fact]
    public async Task Incompatible_units_return_problem_details()
    {
        var response = await _client.GetAsync("/api/v1/conversions?value=1&from=meter&to=kg");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("IncompatibleUnits", json.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Missing_fields_return_validation_error()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/conversions", new { value = 1 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Lists_categories_and_units()
    {
        var categories = await _client.GetFromJsonAsync<string[]>("/api/v1/categories");
        Assert.Contains("length", categories!);

        var units = await _client.GetFromJsonAsync<JsonElement>("/api/v1/units?category=temperature");
        Assert.Equal(3, units.GetArrayLength());
    }

    [Fact]
    public async Task Unknown_category_filter_returns_400()
    {
        var response = await _client.GetAsync("/api/v1/units?category=nope");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
