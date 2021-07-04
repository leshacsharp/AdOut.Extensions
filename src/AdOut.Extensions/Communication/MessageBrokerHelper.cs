using AdOut.Extensions.Communication.Interfaces;
using System;
using System.Linq;

namespace AdOut.Extensions.Communication
{
    public class MessageBrokerHelper : IMessageBrokerHelper
    {
        public string GetQueueName(Type eventType)
        {
            var typeName = GetFullTypeName(eventType).Replace("Event", string.Empty, StringComparison.OrdinalIgnoreCase);
            return FromCamelCaseToSnake(typeName) + "-queue";
        }

        public string GetExchangeName(Type eventType)
        {
            var typeName = GetFullTypeName(eventType).Replace("Event", string.Empty, StringComparison.OrdinalIgnoreCase);
            return FromCamelCaseToSnake(typeName) + "-exchange";
        }   

        private string GetFullTypeName(Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            var index = type.Name.IndexOf('`');
            var quName = type.Name.Substring(0, index);
            foreach (var arg in type.GetGenericArguments())
            {
                quName += GetFullTypeName(arg);
            }
            return quName;
        }

        private string FromCamelCaseToSnake(string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "-" + x.ToString() : x.ToString())).ToLower();
        }
    }
}
