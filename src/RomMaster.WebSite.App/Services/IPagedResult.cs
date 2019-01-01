namespace RomMaster.WebSite.App.Services
{
    public interface IPagedResult<TEntity>
    {
        int TotalPages { get; set; }
        int PageNumber { get; set; }
        int PageSize { get; set; }
        TEntity[] List { get; }
    }
}