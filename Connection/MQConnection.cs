using System;
using RabbitMQ.Client;

namespace MQConnect.RabbitMQ.Connection
{
    public class MQConnection :IMQConnection
    {
        public readonly IConnectionFactory factory;
        public readonly IConnection connection;
        public readonly IModel channel;
        public bool disposed;

        public MQConnection(string url)
        {
            factory = new ConnectionFactory
            {
                Uri = new Uri(url)
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public IModel Channel => channel;

        public void Dispose()
        {
            if (disposed)
                return;
            else
                connection?.Close();

            disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
