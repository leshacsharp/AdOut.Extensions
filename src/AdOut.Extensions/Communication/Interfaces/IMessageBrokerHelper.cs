using System;

namespace AdOut.Extensions.Communication.Interfaces
{
    public interface IMessageBrokerHelper
    {
        string GetQueueName(Type eventType);
        string GetExchangeName(Type eventType);
    }
}
