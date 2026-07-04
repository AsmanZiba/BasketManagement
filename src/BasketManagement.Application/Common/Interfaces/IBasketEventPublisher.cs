namespace BasketManagement.Application.Interfaces;

public interface IBasketEventPublisher
{
    Task PublishBasketItemAddedAsync(long userId, long productId, int quantity);
    Task PublishBasketExpiredAsync(long basketId, long userId);
    Task PublishAsync<T>(string routingKey, T message);
}