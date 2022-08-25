using System;
using System.Collections.Generic;
using System.Text;
using MQConnect.RabbitMQ.Connection;
using RabbitMQ.Client;

namespace MQConnect.RabbitMQ.Publisher
{
    public class MQPublisher :IMQPublisher
    {
        public readonly IMQConnection MQConnection;
        public readonly IModel Channel;
        public readonly string Exchange;
        public bool disposed;
        
        public MQPublisher(IMQConnection mqConnection, string exchange, string type, int timeToLive = 30000)
        {
            MQConnection = mqConnection;
            Channel = MQConnection.Channel;
            Exchange = exchange;
            var ttl = new Dictionary<string, object>
            {
                {"x-message-ttl", timeToLive }
            };

            Channel.ExchangeDeclare(exchange, type, arguments: ttl);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
                Channel?.Close();

            disposed = true;
        }

        public void publish(string body, string routingKey, IDictionary<string, object> messageAttribute, string timeToLive = "30000")
        {
            var message = Encoding.UTF8.GetBytes(body);

            var properties = Channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.Headers = messageAttribute;
            properties.Expiration = timeToLive;

            Channel.BasicPublish(Exchange, routingKey, properties, message);
        }
    }
}
