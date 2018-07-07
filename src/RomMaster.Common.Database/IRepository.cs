namespace RomMaster.Common.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        IQueryable<TEntity> GetAll();

        Task<TEntity> FindAsync(int id);

        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

        Task AddAsync(TEntity entity);

        Task AddRangeAsync(IEnumerable<TEntity> entities);

        IQueryable<TEntity> SqlQuery(FormattableString sql);
    }
}
