using Microsoft.AspNetCore.Blazor;
using RomMaster.Common.Database;
using RomMaster.WebSite.App.Models;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RomMaster.WebSite.App.Services
{
    public class DataSource<TEntity> : IDataSource<GenericViewModel<TEntity>> where TEntity : IEntity
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly string restApiUrl;

        public DataSource(IHttpClientFactory clientFactory, string restApiUrl)
        {
            this.clientFactory = clientFactory;
            this.restApiUrl = restApiUrl;
        }

        public async Task<PagedResult<GenericViewModel<TEntity>>> FetchAsync(PageRequest request)
        {
            var http = clientFactory.CreateClient("default");
            var response = await http.GetJsonAsync<TEntity[]>(restApiUrl);
            return new PagedResult<GenericViewModel<TEntity>>(response.Select(a => new GenericViewModel<TEntity>(a)).ToArray());
        }
    }
}
