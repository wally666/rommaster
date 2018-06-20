namespace RomMaster.Client.Database.Mappings
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using RomMaster.Client.Database.Models;

    internal class RomMapping : IEntityTypeConfiguration<Rom>
    {
        public void Configure(EntityTypeBuilder<Rom> builder)
        {
            //builder.HasKey(a => a.Id);
            //builder.HasAlternateKey(a => a.V1Id);

            builder.Property(a => a.Name).IsRequired();
            builder.Property(a => a.Size).IsRequired();
            builder.Property(a => a.Crc).IsRequired();
            builder.Property(a => a.Md5).IsRequired();
            builder.Property(a => a.Sha1).IsRequired();

            //builder.Property(a => a.CurrentPrice).IsRequired().HasColumnType("DECIMAL(18, 2)");
            //builder.HasMany(a => a.ImageSlots);
            //builder.Property(a => a.CurrentCurrency).HasMaxLength(50).IsRequired();
        }
    }
}
