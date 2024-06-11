using Defra.PTS.Checker.Services.Implementation;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Defra.Trade.Address.V1.ApiClient.Api;
using Defra.Trade.Address.V1.ApiClient.Client;
using Defra.Trade.Common.Security.Authentication.Infrastructure;
using Defra.Trade.Common.Security.Authentication.Interfaces;
using Defra.Trade.Common.Config;

namespace Defra.PTS.Checker.Web.Api.Configuration
{
    public static class ConfigureApi
    {
        public static IServiceCollection AddDefraApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services            
            .Configure<ApimInternalSettings>(configuration.GetSection(ApimInternalSettings.OptionsName));
            services.AddTransient<IApplicationService, ApplicationService>();
            services.AddTransient<IColourService, ColourService>();
            services.AddTransient<IOwnerService, OwnerService>();
            services.AddTransient<ITravelDocumentService, TravelDocumentService>();
            services.AddTransient<IAddressLookupService, AddressLookupService>();
            services.AddTransient<ISailingService, SailingService>();

            services.AddTransient<IPlacesApi>((provider) =>
            new PlacesApi(CreateApiClientConfigurationSettings(provider, configuration)));
            return services;
        }

        private static Trade.Address.V1.ApiClient.Client.Configuration CreateApiClientConfigurationSettings(IServiceProvider provider, IConfiguration configuration)
        {
            var authService = provider.GetService<IAuthenticationService>();
            var apimInternalApisSettings = provider.GetRequiredService<IOptionsSnapshot<ApimInternalSettings>>().Value;
            var authToken = authService!.GetAuthenticationHeaderAsync().Result.ToString();
            
            var config = new Trade.Address.V1.ApiClient.Client.Configuration
            {
                BasePath = configuration.GetValue<string>("AddressApi:BaseUrl"),
                DefaultHeaders = new Dictionary<string, string>
                {
                    { apimInternalApisSettings.AuthorizationHeaderName, authToken },
                    { apimInternalApisSettings.SubscriptionKeyHeaderName, apimInternalApisSettings.SubscriptionKey}
                }
            };
            return config;
        }
    }
}
