using ProductApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ProductStore>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Product API", Version = "v1" }));

builder.WebHost.UseUrls("http://+:5001");

var app = builder.Build();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/openapi.json", "Product API v1");
    c.RoutePrefix = "swagger";
});

app.MapGet("/openapi.json", (Swashbuckle.AspNetCore.Swagger.ISwaggerProvider swaggerProvider) =>
{
    var document = swaggerProvider.GetSwagger("v1");
    using var stringWriter = new StringWriter();
    var jsonWriter = new Microsoft.OpenApi.Writers.OpenApiJsonWriter(stringWriter);
    document.SerializeAsV3(jsonWriter);
    return Results.Text(stringWriter.ToString(), "application/json");
});

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/products", (ProductStore store) => Results.Ok(store.GetAll()));

app.MapGet("/products/{id:int}", (int id, ProductStore store) =>
{
    var product = store.GetById(id);
    return product is null ? Results.NotFound() : Results.Ok(product);
});

app.MapPost("/products", (ProductRequest request, ProductStore store) =>
{
    var error = Validate(request);
    if (error is not null)
    {
        return Results.BadRequest(new { error });
    }

    var created = store.Add(request);
    return Results.Created($"/products/{created.Id}", created);
});

app.MapPut("/products/{id:int}", (int id, ProductRequest request, ProductStore store) =>
{
    var error = Validate(request);
    if (error is not null)
    {
        return Results.BadRequest(new { error });
    }

    var updated = store.Update(id, request);
    return updated is null ? Results.NotFound() : Results.Ok(updated);
});

app.MapDelete("/products/{id:int}", (int id, ProductStore store) =>
    store.Delete(id) ? Results.NoContent() : Results.NotFound());

app.Run();

static string? Validate(ProductRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return "name is required";
    }

    if (request.Price < 0)
    {
        return "price cannot be negative";
    }

    return null;
}

public partial class Program { }
