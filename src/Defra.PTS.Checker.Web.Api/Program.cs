
using Defra.PTS.Checker.Web.Api.Configuration;
using Defra.PTS.Checker.Web.Api.Middleware;
using Defra.PTS.Configuration;
using Defra.Trade.Common.AppConfig;
using Defra.Trade.Common.Security.Authentication.Infrastructure;
using Defra.Trade.Common.Security.AzureKeyVault;
using Defra.Trade.Common.Security.AzureKeyVault.Configuration;
using Microsoft.Azure.Management.Storage.Fluent.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
#if DEBUG
                builder.Configuration.AddJsonFile("appsettings.Development.json", true, true);
#endif

builder.Configuration.AddEnvironmentVariables();


// Add services to the container.
//builder.Services
//    .AddTradeAppConfiguration(builder.Configuration)
//    .AddApimAuthentication(builder.Configuration.GetSection(ApimSettings.InternalApim));

var settings = builder.Configuration.ConfigureTradeAppConfiguration(true, "RemosSignUpService:Sentinel");


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
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PTS Checker API", Version = "v1" });
    c.UseInlineDefinitionsForEnums();

    //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    //c.IncludeXmlComments(xmlFilePath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PTS Checker API v1");
});



app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionHandler>();

app.Run();
