using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Web.Api.Configuration
{
   /// <summary>
   /// Provides extension methods for configuring Azure Key Vault access.
   /// </summary>
   [ExcludeFromCodeCoverage]
   public static class Services
    {
        /// <summary>
        /// Creates a <see cref="SecretClient"/> for the configured Azure Key Vault.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The application configuration containing the Key Vault URI.</param>
        /// <returns>A configured <see cref="SecretClient"/> instance.</returns>
        public static SecretClient AddKeyVault(this IServiceCollection services, IConfiguration configuration)
        {
            var keyVaultUri = configuration["KeyVaultUri"];
            var client = new SecretClient(new Uri(keyVaultUri!), new DefaultAzureCredential());
            return client;
        }
    }
}
