namespace RomMaster.Client.Database.Mappings
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using RomMaster.Client.Database.Models;

    internal class RomMapping : IEntityTypeConfiguration<Rom>
    {
        public void Configure(EntityTypeBuilder<Rom> builder)
        {
            builder.Property(a => a.Name).IsRequired();
            builder.Property(a => a.Size).IsRequired();
            builder.Property(a => a.Crc).IsRequired(false);
            builder.Property(a => a.Md5).IsRequired(false);
            builder.Property(a => a.Sha1).IsRequired(false);

            //builder.Property(a => a.Merge).IsRequired(false);
            //builder.Property(a => a.Status).IsRequired(false);
            //builder.Property(a => a.Date).IsRequired(false);
        }
    }
}
