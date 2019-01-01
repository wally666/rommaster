using System.Threading.Tasks;

namespace RomMaster.WebSite.App.Services
{
    public interface IDataSource<TEntity>
    {
        Task<PagedResult<TEntity>> FetchAsync(PageRequest request);
    }
}
