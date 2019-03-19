using Blazor.FlexGrid.DataSet;
using Blazor.FlexGrid.DataSet.Options;
using Microsoft.Extensions.Http;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RomMaster.WebSite.App.Services
{
    public class LazyDataSet<TEntity> : ILazyDataSetLoader<TEntity>, ILazyDataSetItemManipulator<TEntity> where TEntity : class
    {
        private readonly IHttpClientFactory clientFactory;

        public LazyDataSet(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public async Task<LazyLoadingDataSetResult<TEntity>> GetTablePageData(ILazyLoadingOptions lazyLoadingOptions, IPagingOptions pageableOptions, ISortingOptions sortingOptions)
        {
            var http = clientFactory.CreateClient("default");
            //var response = await http.GetJsonAsync<TEntity[]>(lazyLoadingOptions.DataUri);
            var response = await http.GetAsync(lazyLoadingOptions.DataUri);
            response.EnsureSuccessStatusCode();
            //var items = response.Content.ReadAsAsync<TEntity[]>();
            var items = new TEntity[] { };

            return new LazyLoadingDataSetResult<TEntity>()
            {
                Items = items,
                TotalCount = items.Count()
            };
        }

        public Task<TEntity> SaveItem(TEntity item, ILazyLoadingOptions lazyLoadingOptions)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> DeleteItem(TEntity item, ILazyLoadingOptions lazyLoadingOptions)
        {
            throw new NotImplementedException();
        }
    }
}
