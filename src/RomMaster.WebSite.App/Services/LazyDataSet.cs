using Blazor.FlexGrid.DataSet;
using Blazor.FlexGrid.DataSet.Options;
using Microsoft.AspNetCore.Blazor;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RomMaster.WebSite.App.Services
{
    public class LazyDataSet<TEntity> : ILazyDataSetLoader<TEntity>, ILazyDataSetItemSaver<TEntity> where TEntity : class
    {
        private readonly IHttpClientFactory clientFactory;

        public LazyDataSet(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public async Task<LazyLoadingDataSetResult<TEntity>> GetTablePageData(ILazyLoadingOptions lazyLoadingOptions, IPagingOptions pageableOptions, ISortingOptions sortingOptions)
        {
            var http = clientFactory.CreateClient("default");
            var response = await http.GetJsonAsync<TEntity[]>(lazyLoadingOptions.DataUri);

            return new LazyLoadingDataSetResult<TEntity>()
            {
                Items = response,
                TotalCount = response.Count()
            };
        }

        public Task<TEntity> SaveItem(TEntity item, ILazyLoadingOptions lazyLoadingOptions)
        {
            throw new NotImplementedException();
        }
    }
}
