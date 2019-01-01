using System;

namespace RomMaster.WebSite.App.Components
{
    public abstract class GridColumn<TEntity>
    {
        public string Caption { get; set; }

        public Func<TEntity, object> Bind { get; set; }

        public virtual object ValueFor(TEntity entity)
        {
            return Bind(entity);
        }
    }

    public abstract class GridColumn<TEntity, TValue> : GridColumn<TEntity>
    {
        public new Func<TEntity, TValue> Bind { get; set; }

        public new virtual TValue ValueFor(TEntity entity)
        {
            return Bind(entity);
        }
    }
}