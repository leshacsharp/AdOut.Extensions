using AdOut.Extensions.Communication.Interfaces;
using AdOut.Extensions.Communication.Models;
using AdOut.Extensions.Repositories;
using System.Threading.Tasks;

namespace AdOut.Extensions.Communication
{
    public abstract class BaseReplicationConsumer<TEntity> : BaseConsumer<ReplicationEvent<TEntity>> where TEntity : PersistentEntity
    {
        protected override async Task HandleAsync(ReplicationEvent<TEntity> deliveredEvent)
        {
            var replicationFactory = CreateReplicationFactory();
            var replicationHandler = replicationFactory.CreateReplicationHandler(deliveredEvent.Action);
            await replicationHandler.HandleAsync(deliveredEvent.Data);
        }

        protected abstract IReplicationHandlerFactory<TEntity> CreateReplicationFactory();
    }
}
