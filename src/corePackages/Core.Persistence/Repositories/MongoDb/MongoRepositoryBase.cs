using Core.CrossCuttingConcerns.Exceptions.Types;
using Core.Persistence.Repositories.MongoDb.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Core.Persistence.Repositories.MongoDb;

public abstract class MongoRepositoryBase<TEntity> : IMongoRepository<TEntity>,
    IMongoAsyncRepository<TEntity>
    where TEntity : MongoDbEntity
{
    private readonly IMongoCollection<TEntity> _collection;
    protected string CollectionName { get; set; }

    protected MongoRepositoryBase(MongoConnectionSettings mongoConnectionSetting, string collectionName)
    {
        CollectionName = collectionName;
        ConnectionSettingControl(mongoConnectionSetting);

        var client = mongoConnectionSetting.GetMongoClientSettings() == null
            ? new MongoClient(mongoConnectionSetting.ConnectionString)
            : new MongoClient(mongoConnectionSetting.GetMongoClientSettings());
        var database = client.GetDatabase(mongoConnectionSetting.DatabaseName);
        _collection = database.GetCollection<TEntity>(collectionName);

    }

    public virtual void Add(TEntity entity)
    {
        var options = new InsertOneOptions { BypassDocumentValidation = false };
        _collection.InsertOne(entity, options);
    }

    public virtual IQueryable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate = null)
    {
        return predicate == null
            ? _collection.AsQueryable()
            : _collection.AsQueryable().Where(predicate);
    }

    public virtual TEntity GetById(ObjectId id)
    {
        return _collection.Find(x => x.Id == id).FirstOrDefault();
    }

    public virtual void AddMany(IEnumerable<TEntity> entities)
    {
        var options = new BulkWriteOptions { IsOrdered = false, BypassDocumentValidation = false };
        _collection.BulkWrite((IEnumerable<WriteModel<TEntity>>)entities, options);
    }

    public virtual void Update(ObjectId id, TEntity entity)
    {
        _collection.FindOneAndReplace(x => x.Id == id, entity);
    }

    public virtual void Update(TEntity entity, Expression<Func<TEntity, bool>> predicate)
    {
        _collection.FindOneAndReplace(predicate, entity);
    }

    public virtual void Delete(ObjectId id)
    {
        _collection.FindOneAndDelete(x => x.Id == id);
    }

    public virtual void Delete(TEntity entity)
    {
        _collection.FindOneAndDelete(x => x.Id == entity.Id);
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        var options = new InsertOneOptions { BypassDocumentValidation = false };
        await _collection.InsertOneAsync(entity, options);
    }

    public virtual async Task<IQueryable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null)
    {
        return await Task.Run(() =>
        {
            return predicate == null
                ? _collection.AsQueryable()
                : _collection.AsQueryable().Where(predicate);
        });
    }

    public virtual async Task<TEntity> GetAsync(ObjectId id)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public virtual async Task AddManyAsync(IEnumerable<TEntity> entities)
    {
        var options = new BulkWriteOptions { IsOrdered = false, BypassDocumentValidation = false };
        await _collection.BulkWriteAsync((IEnumerable<WriteModel<TEntity>>)entities, options);
    }

    public virtual async Task UpdateAsync(ObjectId id, TEntity entity)
    {
        await _collection.FindOneAndReplaceAsync(x => x.Id == id, entity);
    }

    public virtual async Task UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> predicate)
    {
        await _collection.FindOneAndReplaceAsync(predicate, entity);
    }

    public virtual async Task DeleteAsync(ObjectId id)
    {
       await _collection.FindOneAndDeleteAsync(x=>x.Id == id);
    }

    public virtual async Task DeleteAsync(TEntity entity)
    {
        await _collection.FindOneAndDeleteAsync(x=>x.Id == entity.Id);
    }

    public bool Any(Expression<Func<TEntity, bool>> predicate = null)
    {
        var data = predicate == null
                ? _collection.AsQueryable()
                : _collection.AsQueryable().Where(predicate);

        return data.FirstOrDefault() != null;
    }

    private void ConnectionSettingControl(MongoConnectionSettings settings)
    {
        if (settings.GetMongoClientSettings() != null &&
            (string.IsNullOrEmpty(CollectionName) || string.IsNullOrEmpty(settings.DatabaseName)))
        {
            throw new BusinessException("Value cannot be null or empty");
        }

        if (string.IsNullOrEmpty(CollectionName) ||
            string.IsNullOrEmpty(settings.ConnectionString) ||
            string.IsNullOrEmpty(settings.DatabaseName))
        {
            throw new BusinessException("Value cannot be null or empty");
        }
    }
}
