using RomMaster.Common.Database;
using System.Threading.Tasks;

namespace RomMaster.WebSite.App.Services
{
    public interface IDataSource<out TEntity> where TEntity : IEntity
    {
        Task<IPagedResult<IEntity>> Fetch(PageRequest request);
        // Task<PagedResult<IEntity>> Fetch(PageRequest request);
    }
}
