using RomMaster.Common.Database;
using RomMaster.WebSite.App.Models;
using System;

namespace RomMaster.WebSite.App.Components
{
    public abstract class GridColumn<TEntity> : IGridColumn where TEntity : IEntity
    {
        public string Caption { get; set; }

        public object Bind { get; set; }
    }
}