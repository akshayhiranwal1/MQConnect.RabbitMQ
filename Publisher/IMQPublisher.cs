using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace MQConnect.RabbitMQ.Publisher
{
    public interface IMQPublisher : IDisposable
    {
        public void publish(string body, string routingKey, IDictionary<string, object> messageAttribute, string timeToLive = "30000");
    }
}
