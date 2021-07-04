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
            try
            {
                var jObject = JObject.Parse(jsonBody);
            }
            catch (JsonReaderException ex)
            {
                //todo: log
                var exceptionMessage = $"{this.GetType().Name} received message with wrong format (body={jsonBody}, exchange={exchange}, routingKey={routingKey})";
                throw new FormatException(exceptionMessage, ex);
            }

            var deliveredEvent = JsonConvert.DeserializeObject<TEvent>(jsonBody);
            await HandleAsync(deliveredEvent);
        }

        protected abstract Task HandleAsync(TEvent deliveredEvent);
    }
}
