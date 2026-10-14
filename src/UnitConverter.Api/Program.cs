using UnitConverter.Api.Domain;
using UnitConverter.Api.Errors;
using UnitConverter.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ConversionExceptionHandler>();

// The catalog is immutable after start-up, so a singleton is safe and cheap.
// Swap this registration for a database/file-backed implementation in the future.
builder.Services.AddSingleton<IUnitCatalog>(_ => BuiltInUnits.Create());
builder.Services.AddSingleton<IConversionService, ConversionService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   // /swagger/v1/swagger.json
    app.UseSwaggerUI(); // /swagger
}

app.MapControllers();

app.Run();

// Required so the integration tests can reference the entry point.
public partial class Program { }
