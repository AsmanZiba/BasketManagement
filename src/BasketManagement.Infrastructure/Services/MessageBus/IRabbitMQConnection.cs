using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketManagement.Infrastructure.Services.MessageBus
{
    public interface IRabbitMQConnection : IDisposable
    {
        IConnection Connection { get; }
        IModel CreateChannel();
        bool IsConnected { get; }
    }
}
