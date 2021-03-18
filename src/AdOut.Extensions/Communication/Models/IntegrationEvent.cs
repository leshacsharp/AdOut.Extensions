using System;

namespace AdOut.Extensions.Communication
{
    public abstract class IntegrationEvent
    {
        public string EventId { get; }
        public DateTime CreatedDateUtc { get; }
        public string Creator { get; set; }

        public IntegrationEvent()
        {
            EventId = Guid.NewGuid().ToString();
            CreatedDateUtc = DateTime.UtcNow;
        }
    }
}
