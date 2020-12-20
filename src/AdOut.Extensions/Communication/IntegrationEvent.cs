using System;

namespace AdOut.Extensions.Communication
{
    public class IntegrationEvent
    {
        public string EventId { get; }

        public DateTime CreatedDateUtc { get; }

        public IntegrationEvent()
        {
            EventId = Guid.NewGuid().ToString();
            CreatedDateUtc = DateTime.UtcNow;
        }
    }
}
