using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MQConnect.RabbitMQ.Consumer
{
    public interface IMQConsumer: IDisposable
    {
        public void Consume(Func<string, IDictionary<string, object>, Task<bool>> callBack);
    }
}
