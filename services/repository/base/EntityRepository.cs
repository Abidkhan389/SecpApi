namespace Paradigm.Service.Repository
{
    using System;
    using System.Data;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Microsoft.EntityFrameworkCore;

    using Paradigm.Data;
    using Paradigm.Contract.Interface;

    using Dapper;

    public class EntityRepository<TEntity> : IEntityRepository<TEntity> where TEntity : class, new()
    {
        private DbContextBase context;
        internal IDbConnection Connection
        {
            get
            {
                return context.Database.GetDbConnection();
            }
        }

        public EntityRepository(DbContextBase context)
        {
            this.context = context;
        }

        // Sync functions

        public IEnumerable<TEntity> All()
        {
            return this.Connection.GetList<TEntity>();
        }

        public IEnumerable<TEntity> FindBy(String predicate)
        {
            return this.Connection.GetList<TEntity>(predicate);
        }

        public IEnumerable<EEntity> FromSql<EEntity>(string query) where EEntity : class, new()
        {
            return this.Connection.Query<EEntity>(query);
        }

        public object Add(TEntity entity)
        {
            return this.Connection.Insert<Guid>(entity);
        }

        public object AddAll(IEnumerable<TEntity> entities)
        {
            object value = null;

            foreach (TEntity entity in entities)
                value = this.Add(entity);

            return value;
        }

        public object Update(TEntity entity)
        {
            return this.Connection.Update(entity);
        }

        public object Delete(TEntity entity)
        {
            return this.Connection.Delete(entity);
        }

        public object Execute(string query)
        {
            return this.Connection.Execute(query);
        }

        // Async functions

        public Task<IEnumerable<TEntity>> AllAsync()
        {
            return Task.Run(() => this.Connection.GetList<TEntity>());
        }

        public Task<IEnumerable<TEntity>> FindByAsync(String predicate)
        {
            return Task.Run(() => this.Connection.GetList<TEntity>(predicate));
        }

        public Task<IEnumerable<EEntity>> FromSqlAsync<EEntity>(string query) where EEntity : class, new()
        {
            return this.Connection.QueryAsync<EEntity>(query);
        }

        public Task<TEntity> AddAsync(TEntity entity)
        {
            return Task.Run(() => this.Connection.Insert<TEntity>(entity));
        }

        public Task<TEntity> AddAllAsync(IEnumerable<TEntity> entities)
        {
            Task<TEntity> value = null;

            foreach (TEntity entity in entities)
                value = this.AddAsync(entity);

            return value;
        }

        public Task<int> UpdateAsync(TEntity entity)
        {
            return Task.Run(() => this.Connection.Update(entity));
        }

        public Task<int> DeleteAsync(TEntity entity)
        {
            return Task.Run(() => this.Connection.Delete<TEntity>(entity));
        }

        public Task<int> ExecuteAsync(string query)
        {
            return this.Connection.ExecuteAsync(query);
        }
    }
}