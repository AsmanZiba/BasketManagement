using MediatR;

namespace BasketManagement.Application.Basket.Events;

public record BasketItemAddedIntegrationEvent(
    long BasketId,
    long UserId,
    long ProductId,
    int Quantity
) : INotification;