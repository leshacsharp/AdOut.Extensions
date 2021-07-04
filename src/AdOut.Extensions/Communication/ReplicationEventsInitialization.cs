using AdOut.Extensions.Communication.Attributes;
using AdOut.Extensions.Communication.Interfaces;
using AdOut.Extensions.Communication.Models;
using AdOut.Extensions.Infrastructure;
using AdOut.Extensions.Repositories;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AdOut.Extensions.Communication
{
    public class ReplicationEventsInitialization : IInitialization
    {
        private readonly IMessageBroker _messageBroker;
        private readonly IEnumerable<IBasicConsumer> _consumers;

        public ReplicationEventsInitialization(
            IMessageBroker messageBroker,
            IEnumerable<IBasicConsumer> consumers)
        {
            _messageBroker = messageBroker;
            _consumers = consumers;
        }

        public Task InitAsync()
        {
            var entitiesToReplicate = AppDomain.CurrentDomain.GetAssemblies()
                                        .SelectMany(a => a.GetTypes())
                                        .Where(t => t == typeof(PersistentEntity) && t.GetCustomAttributes(typeof(ReplicationAttribute), false) != null);

            foreach (var entityType in entitiesToReplicate)
            {
                var eventType = typeof(ReplicationEvent<>).MakeGenericType(entityType);
                _messageBroker.CreateExchange(eventType, ExchangeTypeEnum.Fanout);
                _messageBroker.CreateQueue(eventType);
            }

            foreach (var consumer in _consumers)
            {
                var baseConsumerType = consumer.GetType().BaseType;
                if (baseConsumerType?.GetGenericTypeDefinition() == typeof(BaseReplicationConsumer<>))
                {
                    var entityType = baseConsumerType.GetGenericArguments().Single();
                    var eventType = typeof(ReplicationEvent<>).MakeGenericType(entityType);
                    _messageBroker.Subscribe(eventType, consumer);
                }
            }

            return Task.CompletedTask;
        }
    }
}
