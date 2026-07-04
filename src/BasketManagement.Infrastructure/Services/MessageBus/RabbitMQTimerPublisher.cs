using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;

namespace BasketManagement.Infrastructure.Services.MessageBus;

public class RabbitMQTimerPublisher : BackgroundService
{
    private readonly ILogger<RabbitMQTimerPublisher> _logger;
    private readonly RabbitMQSettings _settings;
    private IModel _channel;

    public RabbitMQTimerPublisher(
        ILogger<RabbitMQTimerPublisher> logger,
        IRabbitMQConnection connection,
        RabbitMQSettings settings)
    {
        _logger = logger;
        _settings = settings;
        _channel = connection.CreateChannel();
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RabbitMQTimerPublisher شروع به کار کرد");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var message = "ExpireCheck";
                var body = Encoding.UTF8.GetBytes(message);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                _channel.BasicPublish(
                    exchange: _settings.ExchangeName,
                    routingKey: "basket.timer",
                    basicProperties: properties,
                    body: body
                );

                _logger.LogInformation("پیام تایمر در {Time} ارسال شد", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ارسال پیام تایمر");
            }

            await Task.Delay(TimeSpan.FromMinutes(_settings.TimerIntervalMinutes), stoppingToken);
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        base.Dispose();
    }
}