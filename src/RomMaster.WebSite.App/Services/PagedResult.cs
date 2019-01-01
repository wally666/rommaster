namespace RomMaster.WebSite.App.Services
{
    public class PagedResult<TEntity> : IPagedResult<TEntity>
    {
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public TEntity[] List { get; set; }

        public PagedResult(TEntity[] data)
        {
            List = data;
        }
    }
}