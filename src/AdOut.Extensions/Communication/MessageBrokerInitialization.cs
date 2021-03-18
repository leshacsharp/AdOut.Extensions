using AdOut.Extensions.Communication.Interfaces;
using AdOut.Extensions.Infrastructure;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdOut.Extensions.Communication
{
    public class MessageBrokerInitialization : IInitialization
    {
        private readonly IMessageBroker _messageBroker;
        private readonly IMessageBrokerHelper _messageBrokerHelper;
        private readonly IEnumerable<IBasicConsumer> _consumers;

        public MessageBrokerInitialization(
            IMessageBroker messageBroker,
            IMessageBrokerHelper messageBrokerHelper,
            IEnumerable<IBasicConsumer> consumers)
        {
            _messageBroker = messageBroker;
            _messageBrokerHelper = messageBrokerHelper;
            _consumers = consumers;
        }

        public Task InitAsync()
        {    
            var eventTypes = AppDomain.CurrentDomain.GetAssemblies()
                                      .SelectMany(a => a.GetTypes())
                                      .Where(t => t.BaseType == typeof(IntegrationEvent));

            foreach (var eventType in eventTypes)
            {
                var exchangeTypeAttr = eventType.GetCustomAttributes(typeof(ExchangeTypeAttribute), false).FirstOrDefault() as ExchangeTypeAttribute;
                var ignoreQueueDeclareAttr = eventType.GetCustomAttributes(typeof(IgnoreQueueDeclareAttribute), false).FirstOrDefault();

                var exchangeType = exchangeTypeAttr != null ? exchangeTypeAttr.Type : ExchangeTypeEnum.Fanout;
                _messageBroker.CreateExchange(eventType, exchangeType);

                if (ignoreQueueDeclareAttr == null)
                {
                    var queue = _messageBrokerHelper.GetQueueName(eventType);
                    _messageBroker.CreateQueue(eventType, queue);
                }
            }

            foreach (var consumer in _consumers)
            {
                var baseConsumerType = consumer.GetType().BaseType;
                if (baseConsumerType?.GetGenericTypeDefinition() == typeof(BaseConsumer<>))
                {
                    var eventType = baseConsumerType.GetGenericArguments().Single();
                    var ignoreQueueDeclareAttr = eventType.GetCustomAttributes(typeof(IgnoreQueueDeclareAttribute), false).FirstOrDefault();
                    if (ignoreQueueDeclareAttr == null)
                    {
                        var queue = _messageBrokerHelper.GetQueueName(eventType);
                        _messageBroker.Subscribe(queue, consumer);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
