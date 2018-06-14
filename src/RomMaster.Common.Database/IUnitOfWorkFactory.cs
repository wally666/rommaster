namespace RomMaster.Common.Database
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
    }
}