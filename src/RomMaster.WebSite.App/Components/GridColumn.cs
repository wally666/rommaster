//using Microsoft.AspNetCore.Blazor;
using System;

namespace RomMaster.WebSite.App.Components
{
    public abstract class GridColumn<TEntity>
    {
        public string Caption { get; set; }

        //public RenderFragment CaptionTemplate { get; set; } //@<p>The time is @DateTime.Now.</p>; // Caption

        public Func<TEntity, object> Bind { get; set; }

        //public RenderFragment RenderCaption()
        //{
        //    return builder =>
        //    {
        //        //builder.OpenElement(1, "h1");
        //        builder.AddContent(1, Caption);
        //        //builder.CloseElement();
        //    };
        //}

        //public RenderFragment RenderValue(TEntity entity)
        //{
        //    return builder =>
        //    {
        //        builder.OpenElement(1, "th");
        //        builder.AddContent(2, ValueFor(entity));
        //        builder.CloseElement();
        //    };
        //}

        public virtual object ValueFor(TEntity entity)
        {
            return Bind(entity);
        }
    }
}