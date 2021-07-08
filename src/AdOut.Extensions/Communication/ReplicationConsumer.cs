using AdOut.Extensions.Communication.Interfaces;
using AdOut.Extensions.Communication.Models;
using AdOut.Extensions.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace AdOut.Extensions.Communication
{
    public class ReplicationConsumer<TEntity> : BaseConsumer<ReplicationEvent<TEntity>> where TEntity : PersistentEntity
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public ReplicationConsumer(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task HandleAsync(ReplicationEvent<TEntity> deliveredEvent)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var replicationFactory = scope.ServiceProvider.GetRequiredService<IReplicationHandlerFactory<TEntity>>();
            var replicationHandler = replicationFactory.CreateReplicationHandler(deliveredEvent.Action);
            await replicationHandler?.HandleAsync(deliveredEvent.Data);
        }
    }
}
