using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace UnitConverter.Api.Errors;

/// <summary>
/// Translates <see cref="ConversionException"/> into an RFC 7807 problem+json 400 response.
/// Any other exception falls through to the default handler (500).
/// </summary>
public sealed class ConversionExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ConversionException conversionException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Conversion failed",
                Detail = conversionException.Message,
                Extensions = { ["code"] = conversionException.Code.ToString() },
            },
        });
    }
}
