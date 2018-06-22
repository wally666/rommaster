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
        // private readonly DbContext context;
        private readonly DbSet<TEntity> dbSet;

        public Repository(DbContext context)
        {
            // this.context = context;
            dbSet = context.Set<TEntity>();
        }

        //public Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entity(TEntity entity)
        //{
        //    return this.context.Entry(entity);
        //}

        public IQueryable<TEntity> All()
        {
            return dbSet; //.AsNoTracking();
        }

        public TEntity Find(int id)
        {
            return dbSet.Find(id);
        }

        public async Task<TEntity> FindAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbSet.FirstOrDefaultAsync(predicate);
        }

        public void Add(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public async Task AddAsync(TEntity entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            dbSet.AddRange(entities);
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}
