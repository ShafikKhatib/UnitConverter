# Unit Conversion API

A small ASP.NET Core Web API (.NET 8, C#) that converts numbers between units of measurement.
Supported categories: **length**, **mass**, **temperature**, and **volume**.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (`dotnet --version` should print `8.x`)

## Run locally

```bash
git clone <repository-url>
cd unit-conversion-api
dotnet run --project src/UnitConverter.Api
```

The API listens on `http://localhost:5080`.
In Development, Swagger UI is at `http://localhost:5080/swagger` (OpenAPI JSON at `/swagger/v1/swagger.json`).
Ready-made requests are in `src/UnitConverter.Api/UnitConverter.Api.http` (VS, Rider, VS Code REST Client).

## Run the tests

```bash
dotnet test
```

## API

| Method | Route                         | Purpose                                   |
|--------|-------------------------------|-------------------------------------------|
| POST   | `/api/v1/conversions`         | Convert a value (JSON body)               |
| GET    | `/api/v1/conversions`         | Convert a value (`?value=&from=&to=`)     |
| GET    | `/api/v1/categories`          | List categories                           |
| GET    | `/api/v1/units?category=`     | List units (optionally by category)       |

### Convert

```bash
curl -X POST http://localhost:5080/api/v1/conversions \
  -H "Content-Type: application/json" \
  -d '{ "value": 100, "fromUnit": "celsius", "toUnit": "fahrenheit" }'
```

```json
{
  "category": "temperature",
  "inputValue": 100,
  "fromUnit": "celsius",
  "fromSymbol": "°C",
  "toUnit": "fahrenheit",
  "toSymbol": "°F",
  "result": 212
}
```

Quick check via query string:

```bash
curl "http://localhost:5080/api/v1/conversions?value=70&from=kg&to=lb"
```

Units are matched case-insensitively by name, symbol or alias (`meter`, `m`, `metres`, `FT`, `feet`, ...).
`category` is optional; when supplied, both units must belong to it.

### Errors

Failures use RFC 7807 `application/problem+json`. Business errors carry a machine-readable `code`:

```json
{
  "title": "Conversion failed",
  "status": 400,
  "detail": "Cannot convert length ('meter') to mass ('kilogram').",
  "code": "IncompatibleUnits"
}
```

Codes: `InvalidValue`, `UnknownUnit`, `UnknownCategory`, `UnitNotInCategory`, `IncompatibleUnits`,
`BelowPhysicalLimit` (e.g. below absolute zero), `ResultOutOfRange`.
Missing/invalid request fields return the standard ASP.NET Core validation problem (400).

## Project structure

```
src/UnitConverter.Api/
  Controllers/   Thin HTTP layer (no conversion logic)
  Contracts/     Request/response DTOs
  Services/      ConversionService - the conversion algorithm
  Domain/        UnitDefinition, UnitCategory, IUnitCatalog, BuiltInUnits (data)
  Errors/        ConversionException + problem+json exception handler
tests/UnitConverter.Api.Tests/   Unit + integration tests (xUnit)
```

## Design decisions and trade-offs

- **Convert through a base unit.** Every unit stores `baseValue = value * factor + offset`
  relative to one base unit per category (meter, kilogram, celsius, liter). A conversion is
  `A -> base -> B`. Adding a unit costs one data line, not a rule per pair, so the design
  scales to hundreds of units (N definitions instead of N x N conversions).
- **One formula for scaling and temperature.** The offset term makes Celsius/Fahrenheit/Kelvin
  ordinary data, with no special-case code in the service.
- **Catalog behind an interface.** Units are hard-coded (as allowed) in `BuiltInUnits`, but the
  service depends only on `IUnitCatalog`. A database or JSON-backed catalog can replace it by
  changing one DI registration. The in-memory catalog validates data at start-up (unknown
  category, duplicate/ambiguous identifier) so bad data fails fast.
- **Units identify their category.** Callers do not have to pass a category; it is inferred and
  mixed-category requests are rejected. `category` stays optional for stricter clients.
- **`double` with rounding to 10 decimals.** Simple and fast; rounding hides binary
  floating-point noise (`212.00000000000003`). `decimal` would be exact for decimal factors
  but cannot represent factors like 5/9 exactly either, so it was not worth the cost. Use
  `decimal` or `BigRational` if financial-grade precision is ever required.
- **Physical limits.** Categories can declare a minimum (temperature: absolute zero), preventing
  nonsense such as -500 °C.
- **Errors as exceptions, translated once.** Expected failures throw `ConversionException`,
  mapped to a 400 problem+json response in one handler, which keeps controllers free of
  error plumbing.
- **Versioned routes** (`/api/v1`) so the contract can evolve.
- **Not included (deliberately):** authentication, persistence, caching, rate limiting.
  Out of scope for this version; the catalog is immutable so caching is trivial later.
- **Ambiguity:** `gallon`, `pint`, `quart`, `cup` and `fluid ounce` are US customary units.

## Adding a unit

Add one line to `Domain/BuiltInUnits.cs`, e.g.:

```csharp
Scale(Length, "nautical mile", "nmi", 1852, "nautical miles"),
```

A new category needs a `UnitCategory` entry plus its units. Tests will catch duplicate identifiers.

## CI

GitHub Actions (`.github/workflows/ci.yml`) restores, builds and tests on every push and pull request.
