namespace RomMaster.Server.WebJob
{
    public interface IAzureSettings
    {
        IAzureKeyVaultSettings Azure { get; }
    }
}
