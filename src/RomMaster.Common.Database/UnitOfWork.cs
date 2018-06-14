namespace RomMaster.Common.Database
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class UnitOfWork : IUnitOfWork
    {
        private bool disposed = false;
        private readonly DbContext context;
        private Dictionary<Type, object> repositories = new Dictionary<Type, object>();

        public UnitOfWork(DbContext context)
        {
            this.context = context;
        }

        public void Commit()
        {
            context.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await context.SaveChangesAsync();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity
        {
            var type = typeof(TEntity);

            if (repositories.ContainsKey(type))
            {
                return repositories[type] as IRepository<TEntity>;
            }

            var repository = new Repository<TEntity>(context);
            repositories.Add(type, repository);
            return repository;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // context.Dispose();
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
