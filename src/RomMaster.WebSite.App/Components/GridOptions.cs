using RomMaster.Common.Database;
using RomMaster.WebSite.App.Services;

namespace RomMaster.WebSite.App.Components
{
    public class GridOptions<TEntity> : IGridOptions where TEntity : IEntity
    {
        public IGridColumn[] Columns { get; set; }

        public IDataSource<IEntity> DataSource { get; set; }
    }
}