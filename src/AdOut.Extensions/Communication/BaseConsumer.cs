using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AdOut.Extensions.Communication
{
    public abstract class BaseConsumer<TEvent> : AsyncDefaultBasicConsumer where TEvent : IntegrationEvent
    {
        public async override Task HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            var jsonBody = Encoding.UTF8.GetString(body.Span);
            var jsonObject = JObject.Parse(jsonBody);

            if (!jsonObject.ContainsKey("ObjectType"))
            {
                var exceptionMessage = $"{this.GetType().Name} received message with wrong schema, missing key 'ObjectType' - (exchange={exchange}, routingKey={routingKey})";
                throw new ArgumentException(exceptionMessage, nameof(body));
            }

            var eventType = jsonObject.GetValue("ObjectType").ToString();
            if (typeof(TEvent).Name != eventType)
            {
                var exceptionMessage = $"{this.GetType().Name} received wrong event={eventType} - (exchange={exchange}, routingKey={routingKey})";
                throw new ArgumentException(exceptionMessage, nameof(body));
            }

            var deliveredEvent = JsonConvert.DeserializeObject<TEvent>(jsonBody);
            await HandleAsync(deliveredEvent);
        }

        protected abstract Task HandleAsync(TEvent deliveredEvent);
    }
}
