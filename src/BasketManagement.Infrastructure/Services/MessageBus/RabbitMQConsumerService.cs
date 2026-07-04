using BasketManagement.Application.Basket.Commands.ExpireBaskets;
using BasketManagement.Application.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BasketManagement.Infrastructure.Services.MessageBus;

public class RabbitMQConsumerService : BackgroundService
{
    private readonly ILogger<RabbitMQConsumerService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRabbitMQConnection _connection;
    private readonly RabbitMQSettings _settings;
    private IModel _channel;

    public RabbitMQConsumerService(
        ILogger<RabbitMQConsumerService> logger,
        IServiceScopeFactory scopeFactory,
        IRabbitMQConnection connection,
        IOptions<RabbitMQSettings> queueSetting)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _connection = connection;
        _settings = queueSetting.Value;
        _channel = _connection.CreateChannel();
        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        _channel.ExchangeDeclare(
            exchange: _settings.ExchangeName,
            type: ExchangeType.Direct,
            durable: true
        );

        _channel.QueueDeclare(
            queue: _settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        _channel.QueueBind(
            queue: _settings.QueueName,
            exchange: _settings.ExchangeName,
            routingKey: "basket.expired"
        );

        _channel.QueueBind(
            queue: _settings.QueueName,
            exchange: _settings.ExchangeName,
            routingKey: "basket.timer"
        );

        _channel.QueueBind(
            queue: _settings.QueueName,
            exchange: _settings.ExchangeName,
            routingKey: "basket.item.added"
        );
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;

            _logger.LogInformation("📩 دریافت پیام: RoutingKey={RoutingKey}", routingKey);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

                switch (routingKey)
                {
                    case "basket.timer":
                        await dispatcher.Send(new ExpireBasketsCommand(), stoppingToken);
                        _logger.LogInformation("✅ دستور ExpireBaskets اجرا شد");
                        break;

                    case "basket.expired":
                        _logger.LogInformation("🕒 سبد منقضی شد: {Message}", message);
                        // پردازش اضافی در صورت نیاز
                        break;

                    case "basket.item.added":
                        _logger.LogInformation("🛒 آیتم به سبد اضافه شد: {Message}", message);
                        // پردازش اضافی در صورت نیاز
                        break;

                    default:
                        _logger.LogWarning("⚠️ RoutingKey ناشناخته: {RoutingKey}", routingKey);
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                        return;
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ خطا در پردازش پیام: {RoutingKey}", routingKey);
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(
            queue: _settings.QueueName,
            autoAck: false,
            consumer: consumer
        );

        _logger.LogInformation("🚀 RabbitMQConsumerService شروع به کار کرد");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        base.Dispose();
    }
}