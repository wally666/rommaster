namespace RomMaster.Common.Database
{
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System;

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly DbSet<TEntity> dbSet;

        public Repository(DbContext context)
        {
            dbSet = context.Set<TEntity>();
        }

        //public Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entity(TEntity entity)
        //{
        //    return this.context.Entry(entity);
        //}

        public IQueryable<TEntity> GetAll()
        {
            return dbSet; //.AsNoTracking();
        }

        public TEntity Find(int id)
        {
            return dbSet.Find(id);
        }

        public Task<TEntity> FindAsync(int id)
        {
            return dbSet.FindAsync(id);
        }

        public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return dbSet.FirstOrDefaultAsync(predicate);
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return dbSet.AnyAsync(predicate);
        }

        public void Add(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public Task AddAsync(TEntity entity)
        {
            return dbSet.AddAsync(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            dbSet.AddRange(entities);
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<TEntity> SqlQuery(FormattableString sql)
        {
            return dbSet.FromSql(sql);
        }
    }
}
