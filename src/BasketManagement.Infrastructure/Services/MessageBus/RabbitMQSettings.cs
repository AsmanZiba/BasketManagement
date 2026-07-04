namespace BasketManagement.Infrastructure.Services.MessageBus;

public class RabbitMQSettings
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string ExchangeName { get; set; } = "basket_events";
    public string QueueName { get; set; } = "basket_expiration_queue";
    public int TimerIntervalMinutes { get; set; } = 1;
    public int ExpirationMinutes { get; set; } = 30;
}
