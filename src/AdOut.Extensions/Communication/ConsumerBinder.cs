using AdOut.Extensions.Communication.Interfaces;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Linq;

namespace AdOut.Extensions.Communication
{
    public class ConsumerBinder : IConsumerBinder
    {
        private readonly IMessageBroker _messageBroker;
        private readonly IEnumerable<IBasicConsumer> _consumers;

        public ConsumerBinder(
            IMessageBroker messageBroker,
            IEnumerable<IBasicConsumer> consumers)
        {
            _messageBroker = messageBroker;
            _consumers = consumers;
        }

        public void Bind()
        {
            foreach (var consumer in _consumers)
            {
                var baseConsumerType = consumer.GetType().BaseType;
                if (baseConsumerType?.GetGenericTypeDefinition() == typeof(BaseConsumer<>))
                {
                    var eventType = baseConsumerType.GetGenericArguments().Single();
                    _messageBroker.Subscribe(eventType, consumer);
                }
            }
        }
    }
}
