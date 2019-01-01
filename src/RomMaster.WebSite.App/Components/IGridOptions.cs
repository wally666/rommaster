using RomMaster.Common.Database;
using RomMaster.WebSite.App.Models;
using RomMaster.WebSite.App.Services;

namespace RomMaster.WebSite.App.Components
{
    public interface IGridOptions
    {
        IGridColumn[] Columns { get; set; }

        //public IDataSource<TEntity> DataSource { get; set; }
    }
}