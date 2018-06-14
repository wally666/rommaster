namespace RomMaster.Client.Database
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var typesToRegister = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => type.GetInterfaces().Any(a => a.IsGenericType && a.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.ApplyConfiguration(configurationInstance);
            }
        }
    }
}