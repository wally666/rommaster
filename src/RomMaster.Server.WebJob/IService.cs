namespace RomMaster.Server.WebJob
{
    using System.Threading.Tasks;

    public interface IService
    {
        Task RunAsync();
    }

    public interface IService<T>
    {
        Task RunAsync(T inputData);
    }
}
