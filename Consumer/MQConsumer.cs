using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MQConnect.RabbitMQ.Connection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MQConnect.RabbitMQ.Consumer
{
    public class MQConsumer : IMQConsumer
    {
        public readonly IMQConnection MQConnection;
        public readonly IModel Channel;
        public readonly string Queue;
        private bool disposed;

        public MQConsumer(IMQConnection mqConnection,
            string queue, string routingKey, string exchange, string type, int timeToLive = 3000, ushort prefetchCount = 10)
        {
            MQConnection = mqConnection;
            Channel = MQConnection.Channel;
            Queue = queue;

            var ttl = new Dictionary<string, object>
            {
                { "x-message-ttl", timeToLive}
            };

            Channel.ExchangeDeclare(exchange, type, arguments: ttl);
            Channel.QueueDeclare(Queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            Channel.QueueBind(Queue, exchange, routingKey);
            Channel.BasicQos(0, prefetchCount, false);
        }

        public void Consume(Func<string, IDictionary<string, object>, Task<bool>> callBack)
        {
            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += async (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                bool success = await callBack.Invoke(message, e.BasicProperties.Headers);
                if (success) Channel.BasicAck(e.DeliveryTag, true);
            };

            Channel.BasicConsume(Queue, true, consumer);
        }

        public void Dispose()
        {
            if (disposed)
                return;

            Channel?.Dispose();

            disposed = true;
            GC.SuppressFinalize(this);

        }
    }
}
