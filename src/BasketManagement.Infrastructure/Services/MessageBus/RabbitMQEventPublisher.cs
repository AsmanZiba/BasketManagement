using BasketManagement.Application.Common.Interfaces;
using BasketManagement.Application.Interfaces;
using BasketManagement.Infrastructure.Services.MessageBus;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BasketManagement.Infrastructure.Services.MessageBus;

public class RabbitMQEventPublisher : IBasketEventPublisher, IScopedDependency, IDisposable
{
    private readonly IRabbitMQConnection _connection;
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQEventPublisher> _logger;
    private readonly IModel _channel;

    public RabbitMQEventPublisher(
        IRabbitMQConnection connection,
        RabbitMQSettings settings,
        ILogger<RabbitMQEventPublisher> logger)
    {
        _connection = connection;
        _settings = settings;
        _logger = logger;
        _channel = _connection.CreateChannel();
        InitializeExchange();
    }

    private void InitializeExchange()
    {
        _channel.ExchangeDeclare(
            exchange: _settings.ExchangeName,
            type: ExchangeType.Direct,
            durable: true
        );
    }

    public async Task PublishBasketItemAddedAsync(long userId, long productId, int quantity)
    {
        var message = new
        {
            UserId = userId,
            ProductId = productId,
            Quantity = quantity,
            EventType = "BasketItemAdded",
            Timestamp = DateTime.UtcNow
        };

        await PublishAsync("basket.item.added", message);
    }

    public async Task PublishBasketExpiredAsync(long basketId, long userId)
    {
        var message = new
        {
            BasketId = basketId,
            UserId = userId,
            EventType = "BasketExpired",
            Timestamp = DateTime.UtcNow
        };

        await PublishAsync("basket.expired", message);
    }

    public async Task PublishAsync<T>(string routingKey, T message)
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";

            _channel.BasicPublish(
                exchange: _settings.ExchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body
            );

            _logger.LogInformation("پیام به RabbitMQ ارسال شد: {RoutingKey}", routingKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ارسال پیام به RabbitMQ");
            throw;
        }

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
    }
}