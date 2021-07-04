using AdOut.Extensions.Repositories;

namespace AdOut.Extensions.Communication.Interfaces
{
    public interface IReplicationHandlerFactory<TEntity> where TEntity : PersistentEntity
    {
        IReplicationHandler<TEntity> CreateReplicationHandler(EventAction action);
    }
}
