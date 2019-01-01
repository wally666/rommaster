using Microsoft.AspNetCore.Blazor.Components;
using RomMaster.WebSite.App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RomMaster.WebSite.App.Components
{
    public class GridBase : BlazorComponent
    {
        [Parameter]
        protected IGridOptions Options { get; set; }

        [Parameter]
        protected IDataSource<Common.Database.IEntity> DataSource { get; set; }

        protected PagedResult<Common.Database.IEntity> PagedResult { get; private set; }

        protected override async Task OnInitAsync()
        {
            var request = new PageRequest();
            // PagedResult = await DataSource.Fetch(request);
        }
    }
}
