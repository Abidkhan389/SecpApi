namespace Paradigm.Contract.Interface
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public interface IEntityRepository<TEntity> where TEntity : class, new()
    {
        // Sync functions
        IEnumerable<TEntity> All();
        IEnumerable<TEntity> FindBy(String predicate);
        IEnumerable<EEntity> FromSql<EEntity>(String query) where EEntity : class, new();
        Object Add(TEntity entity);
        Object AddAll(IEnumerable<TEntity> entities);
        Object Update(TEntity entity);
        Object Delete(TEntity entity);
        Object Execute(String query);

        // Async functions
        Task<IEnumerable<TEntity>> AllAsync();
        Task<IEnumerable<TEntity>> FindByAsync(String predicate);
        Task<IEnumerable<EEntity>> FromSqlAsync<EEntity>(String query) where EEntity : class, new();
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> AddAllAsync(IEnumerable<TEntity> entities);
        Task<int> UpdateAsync(TEntity entity);
        Task<int> DeleteAsync(TEntity entity);
        Task<int> ExecuteAsync(String query);
    }
}