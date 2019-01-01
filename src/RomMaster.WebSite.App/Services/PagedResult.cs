using RomMaster.Common.Database;
using RomMaster.WebSite.App.Models;
using System.Collections.Generic;
using System.Linq;

namespace RomMaster.WebSite.App.Services
{
    public class PagedResult<TEntity> : IPagedResult<TEntity> where TEntity : IEntity
    {
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IList<GenericViewModel<IEntity>> List { get; set; }

        public PagedResult(IEnumerable<TEntity> data)
        {
            List = data.Select(a => new GenericViewModel<IEntity>(a)).ToList();
        }
    }
}