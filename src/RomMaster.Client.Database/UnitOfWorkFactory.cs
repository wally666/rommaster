namespace RomMaster.Client.Database
{
    using RomMaster.Common.Database;

    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly DatabaseContext databaseContext;
        private IUnitOfWork unitOfWork;

        public UnitOfWorkFactory(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public IUnitOfWork Create()
        {
            return unitOfWork ?? (unitOfWork = new UnitOfWork(databaseContext));
        }
    }
}
