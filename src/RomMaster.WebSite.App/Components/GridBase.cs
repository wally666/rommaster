using Microsoft.AspNetCore.Blazor.Components;
using RomMaster.WebSite.App.Services;
using System.Threading.Tasks;

namespace RomMaster.WebSite.App.Components
{
    public class GridBase<TEntity> : BlazorComponent
    {
        [Parameter]
        protected GridOptions<TEntity> Options { get; set; }

        [Parameter]
        protected IDataSource<TEntity> DataSource { get; set; }

        protected IPagedResult<TEntity> PagedResult { get; private set; }

        protected override async Task OnInitAsync()
        {
            var request = new PageRequest();
            PagedResult = await DataSource.FetchAsync(request);
        }
    }
}
