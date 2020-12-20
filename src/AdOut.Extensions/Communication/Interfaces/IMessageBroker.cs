using RabbitMQ.Client;
using System;

namespace AdOut.Extensions.Communication.Interfaces
{
    public interface IMessageBroker
    {
        void Publish(IntegrationEvent integrationEvent);
        void Subscribe(Type eventType, IBasicConsumer eventHandler);
        void Configure();
    }
}
