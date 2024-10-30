using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Web.Api.Configuration
{
   [ExcludeFromCodeCoverage]
   public static class Services
    {
        public static SecretClient AddKeyVault(this IServiceCollection services, IConfiguration configuration)
        {
            var keyVaultUri = configuration["KeyVaultUri"];
            var client = new SecretClient(new Uri(keyVaultUri!), new DefaultAzureCredential());
            return client;
        }
    }
}
