using MongoDB.Bson;
using System.Linq.Expressions;

namespace Core.Persistence.Repositories.MongoDb;

public interface IMongoRepository<TEntity>
     where TEntity : MongoDbEntity
{
    void Add(TEntity entity);
    IQueryable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate = null);
    TEntity GetById(ObjectId id);
    void AddMany(IEnumerable<TEntity> entities);
    void Update(ObjectId id, TEntity entity);
    void Update(TEntity entity, Expression<Func<TEntity, bool>> predicate);
    void Delete(ObjectId id);
    void Delete(TEntity entity);
}
