using Microsoft.AspNetCore.Blazor;
using RomMaster.Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RomMaster.WebSite.App.Services
{
    public class DataSource<TEntity> : IDataSource<IEntity> where TEntity : IEntity
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly string restApiUrl;

        public DataSource(IHttpClientFactory clientFactory, string restApiUrl)
        {
            this.clientFactory = clientFactory;
            this.restApiUrl = restApiUrl;
        }

        public async Task<IPagedResult<IEntity>> Fetch(PageRequest request)
        {
            var http = clientFactory.CreateClient("default");
            var data = await http.GetJsonAsync<TEntity[]>(restApiUrl);
            return new PagedResult<IEntity>(null);
        }
    }
}
