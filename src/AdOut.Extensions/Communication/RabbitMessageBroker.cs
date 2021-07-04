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
            var exchange = _messageBrokerHelper.GetExchangeName(integrationEvent.GetType());
            Publish(integrationEvent, exchange, routingKey, arguments);
        }

        public void Publish(IntegrationEvent integrationEvent, string exchange, string routingKey = null, Dictionary<string, object> arguments = null)
        {
            var channel = _channelManager.GetPublisherChannel();
            var publishProperties = channel.CreateBasicProperties();
            publishProperties.Headers = arguments;

            var eventJson = JsonConvert.SerializeObject(integrationEvent);
            var messageBody = Encoding.UTF8.GetBytes(eventJson);

            channel.BasicPublish(exchange, routingKey, publishProperties, messageBody);
            _channelManager.ReturnPublisherChannel(channel);
        }

        public void Subscribe(Type eventType, IBasicConsumer consumer)
        {
            var queue = _messageBrokerHelper.GetQueueName(eventType);
            Subscribe(queue, consumer);
        }

        public void Subscribe(string queue, IBasicConsumer consumer)
        {
            var channel = _channelManager.GetConsumerChannel();
            channel.BasicConsume(queue, true, consumer);
        }

        public void CreateQueue(Type eventType, string routingKey = null, Dictionary<string, object> arguments = null)
        {
            var queue = _messageBrokerHelper.GetQueueName(eventType);
            var exchange = _messageBrokerHelper.GetExchangeName(eventType);
            CreateQueue(queue, exchange, routingKey, arguments);
        }

        public void CreateQueue(string queue, string exchange, string routingKey = null, Dictionary<string, object> arguments = null)
        {
            var channel = _channelManager.GetPublisherChannel();
            channel.QueueDeclare(queue, true, false, false, null);
            channel.QueueBind(queue, exchange, routingKey, arguments);
            _channelManager.ReturnPublisherChannel(channel);
        }

        public void CreateExchange(Type eventType, ExchangeTypeEnum type)
        {
            var exchange = _messageBrokerHelper.GetExchangeName(eventType);
            CreateExchange(exchange, type);
        }

        public void CreateExchange(string exchange, ExchangeTypeEnum type)
        {
            var channel = _channelManager.GetPublisherChannel();
            channel.ExchangeDeclare(exchange, type.ToString().ToLower());
            _channelManager.ReturnPublisherChannel(channel);
        }   
    }
}
