using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using BasketManagement.Application.Basket.DTOs;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace BasketManagement.Tests.Integration;

public class BasketControllerTests
{
    private readonly WebApplicationFactory<Program> _factory = new WebApplicationFactory<Program>();

    [Fact]
    public async Task GetBasket_ShouldReturnBasket_WhenUserExists()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/basket/1");

        // Assert: بررسی پاسخ
        var status = response.EnsureSuccessStatusCode();
        var basket = await response.Content.ReadFromJsonAsync<BasketDTO>();
        Assert.NotNull(basket);
    }
}