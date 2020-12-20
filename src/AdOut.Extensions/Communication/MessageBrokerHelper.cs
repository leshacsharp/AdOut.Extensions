using AdOut.Extensions.Communication.Interfaces;
using System;
using System.Linq;

namespace AdOut.Extensions.Communication
{
    public class MessageBrokerHelper : IMessageBrokerHelper
    {
        public string GetQueueName(Type eventType)
        {
            var eventName = eventType.Name.Replace("Event", string.Empty);           
            return FromCamelCaseToSnake(eventName) + "-queue";
        }

        public string GetExchangeName(Type eventType)
        {
            var eventName = eventType.Name.Replace("Event", string.Empty);
            return FromCamelCaseToSnake(eventName) + "-exchange";
        }   

        private string FromCamelCaseToSnake(string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "-" + x.ToString() : x.ToString())).ToLower();
        }
    }
}
