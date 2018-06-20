namespace RomMaster.Client.Database.Mappings
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using RomMaster.Client.Database.Models;

    internal class GameMapping : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            //builder.HasKey(a => a.Id);
            //builder.HasAlternateKey(a => a.V1Id);

            builder.HasMany(a => a.Roms);
            builder.Property(a => a.Name).IsRequired();
            builder.Property(a => a.Description).IsRequired(false);
            builder.Property(a => a.Year).IsRequired(false);

            //builder.Property(a => a.CurrentPrice).IsRequired().HasColumnType("DECIMAL(18, 2)");
            //builder.HasMany(a => a.ImageSlots);
            //builder.Property(a => a.CurrentCurrency).HasMaxLength(50).IsRequired();
        }
    }
}
