using System;
using RabbitMQ.Client;

namespace MQConnect.RabbitMQ.Connection
{
    public interface IMQConnection :IDisposable
    {
        public IModel Channel { get; }
    }
}
