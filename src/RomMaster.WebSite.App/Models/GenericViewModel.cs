using RomMaster.Common.Database;

namespace RomMaster.WebSite.App.Models
{
    public class GenericViewModel<TEntity> where TEntity : IEntity
    {
        public bool IsSelected { get; set; }

        public TEntity Item { get; }

        public GenericViewModel(TEntity item)
        {
            Item = item;
        }
    }
}
