using AdOut.Extensions.Communication.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace AdOut.Extensions.Communication
{
    public class RabbitMessageBroker : IMessageBroker
    {
        private readonly IChannelManager _channelManager;
        private readonly IMessageBrokerHelper _messageBrokerHelper;

        public RabbitMessageBroker(
            IChannelManager channelManager,
            IMessageBrokerHelper messageBrokerHelper)
        {
            _channelManager = channelManager;
            _messageBrokerHelper = messageBrokerHelper;
        }

        public void Publish(IntegrationEvent integrationEvent)
        {
            var eventJson = JsonConvert.SerializeObject(integrationEvent, new TypeInfoConverter(integrationEvent.GetType()));
            var messageBody = Encoding.UTF8.GetBytes(eventJson);

            var exchange = _messageBrokerHelper.GetExchangeName(integrationEvent.GetType());
            var routingKey = _messageBrokerHelper.GetQueueName(integrationEvent.GetType());

            var channel = _channelManager.GetPublisherChannel();
            channel.BasicPublish(exchange, routingKey, null, messageBody);

            _channelManager.ReturnPublisherChannel(channel);
        }

        public void Subscribe(Type eventType, IBasicConsumer eventHandler)
        {
            var queue = _messageBrokerHelper.GetQueueName(eventType);
            var channel = _channelManager.GetConsumerChannel();
            channel.BasicConsume(queue, true, eventHandler);
        }

        public void Configure()
        {
            var channel = _channelManager.GetPublisherChannel();

            var eventTypes = AppDomain.CurrentDomain.GetAssemblies()
                                      .SelectMany(a => a.GetTypes())                             
                                      .Where(t => t.BaseType == typeof(IntegrationEvent));

            foreach (var eventType in eventTypes)
            {
                var exchange = _messageBrokerHelper.GetExchangeName(eventType);
                var queue = _messageBrokerHelper.GetQueueName(eventType);

                channel.ExchangeDeclare(exchange, ExchangeType.Fanout);
                channel.QueueDeclare(queue, true, false, false, null);
                channel.QueueBind(queue, exchange, queue, null);
            }

            _channelManager.ReturnPublisherChannel(channel);
        }
    }
}
