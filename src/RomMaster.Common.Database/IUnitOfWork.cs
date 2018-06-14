namespace RomMaster.Common.Database
{
    using System;
    using System.Threading.Tasks;

    public interface IUnitOfWork : IDisposable
    {
        void Commit();

        Task CommitAsync();

        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity;
    }
}
