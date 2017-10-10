namespace RomMaster.Server.WebJob
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.KeyVault;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    public class KeyVaultService
    {
        private readonly IAzureSettings azureKeyVaultSettings;

        public KeyVaultService(IAzureSettings azureKeyVaultSettings)
        {
            this.azureKeyVaultSettings = azureKeyVaultSettings;
        }

        public async Task<string> GetSecretAsync(string name)
        {
            if (!name.EndsWith("-SecretValue"))
            {
                name = name + "-SecretValue";
            }

            using (var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken)))
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("name cannot be null or empty", nameof(name));
                }

                var secret = await keyVaultClient.GetSecretAsync(azureKeyVaultSettings.Azure.KeyVaultBaseUrl, name).ConfigureAwait(false);

                return secret.Value;
            }
        }

        // The method that will be provided to the KeyVaultClient
        private async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            var clientCred = new ClientCredential(azureKeyVaultSettings.Azure.ClientId, azureKeyVaultSettings.Azure.ClientSecret);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the token");
            }

            return result.AccessToken;
        }
    }
}