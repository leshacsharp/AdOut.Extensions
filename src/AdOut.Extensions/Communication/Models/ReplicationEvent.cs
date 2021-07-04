using AdOut.Extensions.Repositories;

namespace AdOut.Extensions.Communication.Models
{
    public class ReplicationEvent<TEntity> : IntegrationEvent where TEntity : PersistentEntity
    {
        public TEntity Data { get; set; }
        public string Information { get; set; }
        public EventAction Action { get; set; }
    }
}
