
using Defra.PTS.Checker.Web.Api.Configuration;
using Defra.PTS.Configuration;
using Defra.Trade.Common.AppConfig;
using Defra.Trade.Common.Security.Authentication.Infrastructure;
using Defra.Trade.Common.Security.AzureKeyVault;
using Defra.Trade.Common.Security.AzureKeyVault.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
builder.Configuration.AddJsonFile("appsettings.json", true, true);

// Add services to the container.
builder.Services
    .AddTradeAppConfiguration(builder.Configuration)
    .AddApimAuthentication(builder.Configuration.GetSection(ApimSettings.InternalApim));

var settings = builder.Configuration.ConfigureTradeAppConfiguration(true, "RemosSignUpService:Sentinel");


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


if (string.IsNullOrEmpty(builder.Configuration.GetConnectionString("sql_db")))
{
    throw new InvalidOperationException("Missing connection string for the database within configuration");
}

builder.Services.AddDefraRepositoriesServices(builder.Configuration.GetConnectionString("sql_db")!);
builder.Services.AddDefraApiServices(builder.Configuration);

//var secretClient = builder.Services.AddKeyVault(builder.Configuration);
//builder.Services.AddSingleton(secretClient);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
