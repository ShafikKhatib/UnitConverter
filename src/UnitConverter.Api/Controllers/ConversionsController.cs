using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using UnitConverter.Api.Contracts;
using UnitConverter.Api.Services;

namespace UnitConverter.Api.Controllers;

[ApiController]
[Route("api/v1/conversions")]
[Produces("application/json")]
public sealed class ConversionsController(IConversionService conversions) : ControllerBase
{
    /// <summary>Convert a number from one unit to another (JSON body).</summary>
    [HttpPost]
    [ProducesResponseType<ConvertResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<ConvertResponse> Convert([FromBody] ConvertRequest request)
    {
        var result = conversions.Convert(request.Value!.Value, request.FromUnit!, request.ToUnit!, request.Category);
        return Ok(ConvertResponse.From(result));
    }

    /// <summary>Convert a number from one unit to another (query string, handy for quick checks).</summary>
    [HttpGet]
    [ProducesResponseType<ConvertResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<ConvertResponse> ConvertFromQuery(
        [FromQuery, Required] double? value,
        [FromQuery, Required] string from,
        [FromQuery, Required] string to,
        [FromQuery] string? category = null)
    {
        var result = conversions.Convert(value!.Value, from, to, category);
        return Ok(ConvertResponse.From(result));
    }
}
