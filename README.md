# C# — Product API (ASP.NET Core Minimal API)

A minimal, in-memory Product CRUD API built with ASP.NET Core Minimal APIs.

## Prerequisites

- .NET SDK 8.0+

## Restore

```bash
dotnet restore
```

## Run

```bash
dotnet run
```

The API listens on `http://localhost:5001`.

## Test

```bash
dotnet test
```

## API base URL

`http://localhost:5001`

## OpenAPI / Swagger

- OpenAPI JSON: `http://localhost:5001/openapi.json`
- Swagger UI: `http://localhost:5001/swagger`

Generated via `Swashbuckle.AspNetCore` — no hand-written spec.

## Notes

- Storage is a single in-memory `ConcurrentDictionary<int, Product>` inside `ProductStore`, seeded with 2 sample products at startup.
- IDs are generated with `Interlocked.Increment`.
- Validation (`name` required, `price >= 0`) is explicit inline in `Program.cs`.
- No Entity Framework, no controllers, no service layers — endpoints are defined directly in `Program.cs`.
