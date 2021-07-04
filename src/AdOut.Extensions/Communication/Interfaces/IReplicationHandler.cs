using AdOut.Extensions.Repositories;
using System.Threading.Tasks;

namespace AdOut.Extensions.Communication.Interfaces
{
    public interface IReplicationHandler<TEntity> where TEntity : PersistentEntity
    {
        Task HandleAsync(TEntity entity);
    }
}
