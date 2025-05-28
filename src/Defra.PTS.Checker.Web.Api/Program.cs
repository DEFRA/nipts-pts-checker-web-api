
using Defra.PTS.Checker.Web.Api.Configuration;
using Defra.PTS.Checker.Web.Api.Middleware;
using Defra.PTS.Configuration;
using Defra.Trade.Common.Api.Health;
using Defra.Trade.Common.AppConfig;
using Defra.Trade.Common.Security.Authentication.Infrastructure;
using Defra.Trade.Common.Security.AzureKeyVault;
using Defra.Trade.Common.Security.AzureKeyVault.Configuration;
using Microsoft.Azure.Management.Storage.Fluent.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
#if DEBUG
                builder.Configuration.AddJsonFile("appsettings.Development.json", true, true);
#endif

builder.Configuration.AddEnvironmentVariables();

builder.Configuration.ConfigureTradeAppConfiguration();
//builder.Configuration.ConfigureTradeAppConfiguration(true, "RemosSignUpService:Sentinel");


// Add services to the container.
builder.Services
    .AddTradeAppConfiguration(builder.Configuration)
    .AddApimAuthentication(builder.Configuration.GetSection(ApimSettings.InternalApim));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connection = builder.Configuration.GetConnectionString("sql_db");
if (string.IsNullOrEmpty(connection))
{
    throw new InvalidOperationException("Missing connection string for the database within configuration");
}


builder.Services.AddDefraRepositoriesServices(connection);
builder.Services.AddDefraApiServices(builder.Configuration);
builder.Services.AddHealthChecks();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PTS Checker API", Version = "v1" });
    c.UseInlineDefinitionsForEnums();
});

builder.Services.AddApplicationInsightsTelemetry();

var origins = new string[] { "https://pre-check-a-pet-from-gb-to-ni.azure.defra.cloud/",
                "https://tst-check-a-pet-from-gb-to-ni.azure.defra.cloud/",
                "https://dev-check-a-pet-from-gb-to-ni.azure.defra.cloud/",
                 "https://check-a-pet-from-gb-to-ni.service.gov.uk/" };

#if DEBUG
origins = origins.Append("http://localhost:5000").ToArray();
#endif

builder.Services.AddCors(options =>
{
    options.AddPolicy("RestrictedPolicy", policy =>
    {
        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PTS Checker API v1");
});

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseTradeHealthChecks();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionHandler>();

app.Run();


[ExcludeFromCodeCoverage]
static partial class Program
{

}