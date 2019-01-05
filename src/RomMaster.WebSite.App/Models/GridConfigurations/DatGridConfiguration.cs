using Blazor.FlexGrid.Components.Configuration;
using Blazor.FlexGrid.Components.Configuration.Builders;
using RomMaster.Client.Database.Models;

namespace RomMaster.WebSite.App.Models.GridConfigurations
{
    public class DatGridConfiguration : IEntityTypeConfiguration<Dat>
    {
        public void Configure(EntityTypeBuilder<Dat> builder)
        {
            builder.Property(a => a.Id).HasCaption("Id").IsSortable().IsVisible(true);
            builder.Property(a => a.Name).HasCaption("Name").IsSortable().IsVisible(true);
            builder.Property(a => a.Version).HasCaption("Version").IsSortable().IsVisible(true);
            builder.Property(a => a.Date).HasCaption("Date").IsSortable().IsVisible(true).HasValueFormatter(a => a.HasValue ? a.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty);
            builder.Property(a => a.Category).HasCaption("Category").IsSortable().IsVisible(true);
            builder.Property(a => a.Description).HasCaption("Description").IsSortable().IsVisible(true);
            builder.Property(a => a.Author).HasCaption("Author").IsSortable().IsVisible(true);
            builder.Property(a => a.File).HasCaption("File").IsSortable().IsVisible(true).HasValueFormatter(a => a.Path);
            builder.Property(a => a.Games).HasCaption("Games").IsSortable().IsVisible(true).HasValueFormatter(a => a.Count.ToString());

            builder.UseCssClasses(conf =>
            {
                conf.Table = "app-table";
                conf.TableBody = "app-table-body";
                conf.TableCell = "app-table-cell";
                conf.TableHeader = "app-table-header";
                conf.TableHeaderCell = "app-table-header-cell";
                conf.TableHeaderRow = "app-table-header-row";
                conf.TableRow = "app-table-row";
            });

            // builder.AllowInlineEdit();
            //
            //builder.Property(e => e.Date)
            //    .HasCaption("Date")
            //    //.HasWritePermissionRestriction(perm => perm.IsInRole("TestRole1"))
            //    .HasValueFormatter(d => d.ToShortDateString());

            //builder.Property(e => e.Summary)
            //    .HasCaption("MySummary")
            //    .HasOrder(1)
            //    //.HasValueFormatter(s => $"{s}!");
            //    .HasCompositeValueFormatter(f => $"{f.Summary} <button>{f.TemperatureC}</button> {f.TemperatureF}");

            //builder.Property(e => e.TemperatureC)
            //    .IsSortable();

            //builder.Property(e => e.TemperatureF)
            //    .IsSortable();
        }
    }
}