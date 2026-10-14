using Microsoft.AspNetCore.Mvc;
using UnitConverter.Api.Contracts;
using UnitConverter.Api.Domain;
using UnitConverter.Api.Errors;

namespace UnitConverter.Api.Controllers;

[ApiController]
[Route("api/v1")]
[Produces("application/json")]
public sealed class UnitsController(IUnitCatalog catalog) : ControllerBase
{
    /// <summary>List the supported conversion categories.</summary>
    [HttpGet("categories")]
    public ActionResult<IEnumerable<string>> GetCategories() =>
        Ok(catalog.Categories.Select(c => c.Name).Order());

    /// <summary>List supported units, optionally filtered by category.</summary>
    [HttpGet("units")]
    [ProducesResponseType<IEnumerable<UnitDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<IEnumerable<UnitDto>> GetUnits([FromQuery] string? category = null)
    {
        IEnumerable<UnitDefinition> units = catalog.Units;

        if (!string.IsNullOrWhiteSpace(category))
        {
            if (!catalog.TryGetCategory(category, out var found))
            {
                throw new ConversionException(ConversionError.UnknownCategory, $"Unknown category '{category}'.");
            }

            units = units.Where(u => string.Equals(u.Category, found.Name, StringComparison.OrdinalIgnoreCase));
        }

        return Ok(units.Select(UnitDto.From));
    }
}
