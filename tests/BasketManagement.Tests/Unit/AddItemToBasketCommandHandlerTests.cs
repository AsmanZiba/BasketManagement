using Moq;
using BasketManagement.Application.Interfaces;
using BasketManagement.Domain.Entities;
using BasketManagement.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using BasketManagement.Application.Basket.Commands.AddItemToBasket;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace BasketManagement.Tests.Unit;

public class AddItemToBasketCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ShouldAddItemAndInvalidateCache()
    {
        // Arrange
        var basket = new Basket(1);
        var repoMock = new Mock<IBasketRepository>();
        repoMock.Setup(r => r.GetActiveBasketByUserIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(basket);
        var uowMock = new Mock<IUnitOfWork>();
        var cacheMock = new Mock<IBasketCacheService>();
        var eventPubMock = new Mock<IBasketEventPublisher>();
        var logMock = new Mock<ILogger<AddItemToBasketCommandHandler>>();

        var handler = new AddItemToBasketCommandHandler(
            repoMock.Object,
            uowMock.Object,
            cacheMock.Object,
            eventPubMock.Object,
            logMock.Object);

        var command = new AddItemToBasketCommand(1, new AddBasketItemDTO { ProductId = 10, Quantity = 2 });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(basket.Items);
        cacheMock.Verify(c => c.RemoveBasketAsync(1), Times.Once);
        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}