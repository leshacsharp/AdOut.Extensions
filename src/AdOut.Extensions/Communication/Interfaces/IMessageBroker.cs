using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace AdOut.Extensions.Communication.Interfaces
{
    public interface IMessageBroker
    {
        void CreateExchange(Type eventType, ExchangeTypeEnum type);
        void CreateQueue(Type eventType, string queue, string routingKey = null, Dictionary<string, object> arguments = null);
        void Publish(IntegrationEvent integrationEvent, string routingKey = null, Dictionary<string, object> arguments = null);
        void Subscribe(string queue, IBasicConsumer consumer);
    }
}
