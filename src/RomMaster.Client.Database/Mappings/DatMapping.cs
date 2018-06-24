namespace RomMaster.Client.Database.Mappings
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using RomMaster.Client.Database.Models;

    internal class DatMapping : IEntityTypeConfiguration<Dat>
    {
        public void Configure(EntityTypeBuilder<Dat> builder)
        {
            //builder.HasKey(a => a.Id);
            //builder.HasAlternateKey(a => a.V1Id);

            builder.HasMany(a => a.Games);
            builder.Property(a => a.Version).IsRequired();
            builder.Property(a => a.Date).IsRequired(false);
            builder.Property(a => a.Name).IsRequired();
            builder.Property(a => a.Description).IsRequired();
            builder.Property(a => a.Author).IsRequired(false);
            builder.Property(a => a.Category).IsRequired(false);

            builder.HasIndex(a => new {a.Name, a.Version}).IsUnique(true);

            //builder.Property(a => a.CurrentPrice).IsRequired().HasColumnType("DECIMAL(18, 2)");
            //builder.HasMany(a => a.ImageSlots);
            //builder.Property(a => a.CurrentCurrency).HasMaxLength(50).IsRequired();
        }
    }
}
