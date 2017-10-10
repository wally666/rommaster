namespace RomMaster.Server.WebJob
{
    using Microsoft.Azure.WebJobs.Extensions.SendGrid;

    public class AppOptions : IAzureSettings
    {
        public IAzureKeyVaultSettings Azure { get; } = new AzureKeyVaultSettings();

        public SendGridConfiguration SendGrid { get; } = new SendGridConfiguration();
    }
}
