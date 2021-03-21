using AdOut.Extensions.Communication.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
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
        
        public void Publish(IntegrationEvent integrationEvent, string routingKey = null, Dictionary<string, object> arguments = null)
        {
            var channel = _channelManager.GetPublisherChannel();
            var publishProperties = channel.CreateBasicProperties();

            var eventJson = JsonConvert.SerializeObject(integrationEvent, new TypeInfoConverter(integrationEvent.GetType()));
            var messageBody = Encoding.UTF8.GetBytes(eventJson);

            var exchange = _messageBrokerHelper.GetExchangeName(integrationEvent.GetType());
            publishProperties.Headers = arguments;

            channel.BasicPublish(exchange, routingKey, publishProperties, messageBody);
            _channelManager.ReturnPublisherChannel(channel);
        }

        public void Subscribe(string queue, IBasicConsumer consumer)
        {
            var channel = _channelManager.GetConsumerChannel();
            channel.BasicConsume(queue, true, consumer);
        }

        public void CreateQueue(Type eventType, string queue, string routingKey, Dictionary<string, object> arguments = null)
        {
            var channel = _channelManager.GetPublisherChannel();

            var exchange = _messageBrokerHelper.GetExchangeName(eventType);
            channel.QueueDeclare(queue, true, false, false, null);
            channel.QueueBind(queue, exchange, routingKey, arguments);

            _channelManager.ReturnPublisherChannel(channel);
        }

        public void CreateExchange(Type eventType, ExchangeTypeEnum type)
        {
            var channel = _channelManager.GetPublisherChannel();

            var exchange = _messageBrokerHelper.GetExchangeName(eventType);
            channel.ExchangeDeclare(exchange, type.ToString().ToLower());

            _channelManager.ReturnPublisherChannel(channel);
        }
    }
}
