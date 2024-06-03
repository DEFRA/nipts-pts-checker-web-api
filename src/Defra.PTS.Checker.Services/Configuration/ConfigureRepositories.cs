using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories;
using Defra.PTS.Checker.Repositories.Implementation;
using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace Defra.PTS.Configuration
{
    public static class ConfigureRepositories
    {
        public static IServiceCollection AddDefraRepositoriesServices(this IServiceCollection services,string conn)
        {
            services.AddDbContext<CommonDbContext>((context) =>
            {
                context.UseSqlServer(conn);
            });
            services.AddScoped<DbContext, CommonDbContext>();
            services.AddTransient<IApplicationRepository, ApplicationRepository>();
            services.AddTransient<ISailingRepository, SailingRepository>();
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

            return services;
        }
    }
}
