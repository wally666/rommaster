namespace RomMaster.Server.WebJob
{
    public class AzureKeyVaultSettings : IAzureKeyVaultSettings
    {
        public string KeyVaultBaseUrl { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}
