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
    public class MessageBrokerInitialization : IInitialization
    {
        private readonly IMessageBroker _messageBroker;
        private readonly IEnumerable<IBasicConsumer> _consumers;

        public MessageBrokerInitialization(
            IMessageBroker messageBroker,
            IEnumerable<IBasicConsumer> consumers)
        {
            _messageBroker = messageBroker;
            _consumers = consumers;
        }

        public Task InitAsync()
        {
            InitSimpleEvents();
            InitReplicationEvents();
            BindConsumers();
            return Task.CompletedTask;
        }


        private void InitSimpleEvents()
        {
            var eventTypes = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(a => a.GetTypes())
                            .Where(t => t.BaseType == typeof(IntegrationEvent) && !t.IsGenericType);

            foreach (var eventType in eventTypes)
            {
                var eventDeclarationAttr = eventType.GetCustomAttributes(typeof(EventDeclarationAttribute), false).FirstOrDefault() as EventDeclarationAttribute;
                var ignoreEventDeclarationAttr = eventType.GetCustomAttributes(typeof(IgnoreEventDeclarationAttribute), false).FirstOrDefault();
                if (ignoreEventDeclarationAttr != null)
                {
                    continue;
                }

                var exchangeType = eventDeclarationAttr != null ? eventDeclarationAttr.ExchangeType : ExchangeTypeEnum.Fanout;
                _messageBroker.CreateExchange(eventType, exchangeType);
                _messageBroker.CreateQueue(eventType);
            }
        }

        private void InitReplicationEvents()
        {
            var entitiesToReplicate = AppDomain.CurrentDomain.GetAssemblies()
                                     .SelectMany(a => a.GetTypes())
                                     .Where(t => t == typeof(PersistentEntity) && t.GetCustomAttributes(typeof(ReplicationAttribute), false).Any());

            foreach (var entityType in entitiesToReplicate)
            {
                var eventType = typeof(ReplicationEvent<>).MakeGenericType(entityType);
                _messageBroker.CreateExchange(eventType, ExchangeTypeEnum.Fanout);
                _messageBroker.CreateQueue(eventType);
            }
        }

        private void BindConsumers()
        {
            foreach (var consumer in _consumers)
            {
                var baseConsumerType = consumer.GetType().BaseType;
                if (baseConsumerType?.GetGenericTypeDefinition() == typeof(BaseConsumer<>))
                {
                    var eventType = baseConsumerType.GetGenericArguments().Single();
                    var ignoreEventDeclarationAttr = eventType.GetCustomAttributes(typeof(IgnoreEventDeclarationAttribute), false).FirstOrDefault();
                    if (ignoreEventDeclarationAttr != null)
                    {
                        continue;
                    }
                    _messageBroker.Subscribe(eventType, consumer);
                }
            }
        }
    }
}
