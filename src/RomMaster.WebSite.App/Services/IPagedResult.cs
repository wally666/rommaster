using RomMaster.Common.Database;
using RomMaster.WebSite.App.Models;
using System.Collections.Generic;
using System.Linq;

namespace RomMaster.WebSite.App.Services
{
    public interface IPagedResult<out TEntity> where TEntity : IEntity
    {
        int TotalPages { get; set; }
        int PageNumber { get; set; }
        int PageSize { get; set; }
        IList<GenericViewModel<IEntity>> List { get; set; }
    }
}