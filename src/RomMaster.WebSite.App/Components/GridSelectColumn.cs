using RomMaster.Common.Database;
using RomMaster.WebSite.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RomMaster.WebSite.App.Components
{
    public class GridSelectColumn<TEntity> : GridColumn<TEntity> where TEntity : IEntity
    {
    }
}
