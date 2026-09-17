using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ProductApi;
using Xunit;

namespace ProductApi.Tests;

public class ProductApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_ReturnsOk()
    {
        var response = await _client.GetAsync("/products");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostThenGet_ReturnsCreatedProduct()
    {
        var request = new ProductRequest("Test Product", "A product for testing", 19.99m);

        var postResponse = await _client.PostAsJsonAsync("/products", request);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var created = await postResponse.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(created);
        Assert.Equal("Test Product", created!.Name);

        var getResponse = await _client.GetAsync($"/products/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetUnknownProduct_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/products/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostInvalidProduct_ReturnsBadRequest()
    {
        var request = new ProductRequest("", "Missing name", 10m);
        var response = await _client.PostAsJsonAsync("/products", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNoContent()
    {
        var request = new ProductRequest("Temp Product", "Will be deleted", 5m);
        var postResponse = await _client.PostAsJsonAsync("/products", request);
        var created = await postResponse.Content.ReadFromJsonAsync<Product>();

        var deleteResponse = await _client.DeleteAsync($"/products/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}
