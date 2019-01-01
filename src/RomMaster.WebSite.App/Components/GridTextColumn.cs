using RomMaster.Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RomMaster.WebSite.App.Components
{
    public class GridTextColumn<TEntity> : GridColumn<TEntity> where TEntity : IEntity
    {
    }
}
