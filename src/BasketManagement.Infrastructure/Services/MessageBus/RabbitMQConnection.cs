using BasketManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BasketManagement.Infrastructure.Services.MessageBus;

public class RabbitMQConnection : IRabbitMQConnection, ISingletonDependency, IDisposable
{
    private IConnection _connection;
    private readonly RabbitMQSettings _queueSetting;
    private readonly object _lock = new();
    private readonly ILogger<RabbitMQConnection> _logger;
    private bool _disposed;

    public RabbitMQConnection(IOptions<RabbitMQSettings> queueSetting, ILogger<RabbitMQConnection> logger)
    {
        _queueSetting = queueSetting.Value;
        Connect();
        _logger = logger;
    }

    public IConnection Connection => _connection;
    public bool IsConnected => _connection != null && _connection.IsOpen;

    private void Connect()
    {
        lock (_lock)
        {
            if (IsConnected) return;

            var factory = new ConnectionFactory
            {
                HostName = _queueSetting.HostName,
                Port = _queueSetting.Port,
                UserName = _queueSetting.UserName,
                Password = _queueSetting.Password,
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            _connection = factory.CreateConnection();
            _connection.ConnectionShutdown += (s, e) =>
                _logger.LogTrace($"RabbitMQ Connection Shutdown: {e.ReplyText}");
        }
    }

    public IModel CreateChannel()
    {
        if (!IsConnected) Connect();
        return _connection.CreateModel();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _connection?.Close();
        _connection?.Dispose();
    }
}