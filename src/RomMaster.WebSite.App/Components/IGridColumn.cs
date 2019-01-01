using RomMaster.Common.Database;
using RomMaster.WebSite.App.Models;
using System;

namespace RomMaster.WebSite.App.Components
{
    public interface IGridColumn
    {
        string Caption { get; set; }

        object Bind { get; set; }
    }
}