using Defra.PTS.Checker.Repositories;
using Defra.PTS.Checker.Repositories.Implementation;
using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class ConfigureRepositories
    {
        public static IServiceCollection AddDefraRepositoriesServices(this IServiceCollection services,string conn)
        {
            services.AddDbContext<CommonDbContext>((context) =>
            {
                context.UseSqlServer(conn);
            });
            services.AddScoped<DbContext, CommonDbContext>();
            services.AddTransient<ICheckerRepository, CheckerRepository>();
            services.AddTransient<IPetRepository, PetRepository>();
            services.AddTransient<IBreedRepository, BreedRepository>();
            services.AddTransient<IColourRepository, ColourRepository>();
            services.AddTransient<ITravelDocumentRepository, TravelDocumentRepository>();
            services.AddTransient<IApplicationRepository, ApplicationRepository>();
            services.AddTransient<ISailingRepository, SailingRepository>();
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

            return services;
        }
    }
}
