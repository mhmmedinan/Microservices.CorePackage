using MongoDB.Bson;
using System.Linq.Expressions;

namespace Core.Persistence.Repositories.MongoDb;

public interface IMongoAsyncRepository<TEntity>
    where TEntity : MongoDbEntity
{
    Task AddAsync(TEntity entity);
    Task<IQueryable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null);
    Task<TEntity> GetAsync(ObjectId id);
    Task AddManyAsync(IEnumerable<TEntity> entities);
    Task UpdateAsync(ObjectId id, TEntity entity);
    Task UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> predicate);
    Task DeleteAsync(ObjectId id);
    Task DeleteAsync(TEntity entity);
    bool Any(Expression<Func<TEntity, bool>> predicate = null);
}
