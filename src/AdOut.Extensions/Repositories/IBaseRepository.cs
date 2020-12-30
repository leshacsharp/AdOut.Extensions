using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AdOut.Extensions.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : PersistentEntity
    {
        void Create(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        ValueTask<TEntity> GetByIdAsync(params object[] id);
        IQueryable<TEntity> Read(Expression<Func<TEntity, bool>> predicate);
    }
}
