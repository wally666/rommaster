namespace RomMaster.Server.WebJob
{
    public class AppSecrets
    {
        public string WebJobStorageConnectionString { get; }

        public AppSecrets(KeyVaultService keyVaultService)
        {
            WebJobStorageConnectionString = keyVaultService.GetSecretAsync("WebJobStorageConnectionString").Result;
        }
    }
}