using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Defra.PTS.Checker.Web.Api.Configuration
{
    public static class Services
    {
        public static SecretClient AddKeyVault(this IServiceCollection services, IConfiguration configuration)
        {
            var keyVaultUri = configuration["KeyVaultUri"];
            var client = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
            return client;
        }
    }
}
