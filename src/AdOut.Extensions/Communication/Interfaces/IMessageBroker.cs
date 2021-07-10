using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace AdOut.Extensions.Communication.Interfaces
{
    public interface IMessageBroker
    {
        void CreateExchange(Type eventType, ExchangeTypeEnum type);
        void CreateExchange(string exchange, ExchangeTypeEnum type);
        void CreateQueue(Type eventType, string routingKey = "", Dictionary<string, object> arguments = null);
        void CreateQueue(string queue, string exchange, string routingKey = "", Dictionary<string, object> arguments = null);
        void Publish(IntegrationEvent integrationEvent, string routingKey = "", Dictionary<string, object> arguments = null);
        void Publish(IntegrationEvent integrationEvent, string exchange, string routingKey = "", Dictionary<string, object> arguments = null);
        void Subscribe(Type eventType, IBasicConsumer consumer);
        void Subscribe(string queue, IBasicConsumer consumer);
    }
}
