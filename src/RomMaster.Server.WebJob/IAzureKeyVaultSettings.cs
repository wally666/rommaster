namespace RomMaster.Server.WebJob
{
    public interface IAzureKeyVaultSettings
    {
        string KeyVaultBaseUrl { get; }
        string ClientId { get; }
        string ClientSecret { get; }
    }
}
