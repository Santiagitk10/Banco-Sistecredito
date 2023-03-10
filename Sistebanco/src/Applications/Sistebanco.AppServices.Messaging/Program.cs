using credinet.comun.api;
using credinet.comun.api.Swagger.Extensions;
using credinet.exception.middleware;
using EntryPoints.ServiceBus.Clientes;
using Helpers.ObjectsUtils;
using Helpers.ObjectsUtils.Setting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.PlatformAbstractions;
using SC.Configuration.Provider.Mongo;
using Serilog;
using System.IO;
using System.Linq;
using Tiendas.AppServices.Messaging.Extensions;
using Tiendas.AppServices.Messaging.Extensions.Health;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

#region Host Configuration

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonProvider();

//HACK: Para usar fuera de Siste.
//builder.Configuration.AddKeyVaultProvider();

builder.Host.UseSerilog((ctx, lc) => lc
       .WriteTo.Console()
       .ReadFrom.Configuration(ctx.Configuration));

#endregion Host Configuration

builder.Services.Configure<ConfiguradorAppSettings>(builder.Configuration.GetRequiredSection(nameof(ConfiguradorAppSettings)));
ConfiguradorAppSettings appSettings = builder.Configuration.GetSection(nameof(ConfiguradorAppSettings)).Get<ConfiguradorAppSettings>();
//HACK: Para usar fuera de Siste.
//Secrets secrets = builder.Configuration.ResolveSecrets<Secrets>();
Secrets secretos = builder.Configuration.GetSection(nameof(Secrets)).Get<Secrets>();
string country = EnvironmentHelper.GetCountryOrDefault(appSettings.DefaultCountry);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddMongoProvider(
    nameof(MongoConfigurationProvider), secretos.MongoConnection, country);

#region Service Configuration

string policyName = "cors";
builder.Services
    .RegisterMongo(secretos.MongoConnection, $"{appSettings.Database}_{country}")
    .RegisterAsyncGateways(secretos.ServicesBusConnection)
    .RegisterServices()
    .RegisterSubscriptions();

builder.Services
    .AddHealthChecks()
    .AddMongoDb(secretos.MongoConnection, name: "MongoDB");

#endregion Service Configuration

WebApplication app = builder.Build();

var appClienteCommandSubscription =
    app.Services.GetRequiredService<IClienteCommandSubscription>();

appClienteCommandSubscription.SubscribeAsync().Wait();

// Enable middleware to serve generated Swagger as a JSON endpoint.

app.UseRouting();
app.UseCors(policyName);
app.UseStaticFiles();
app.ServiceHealthChecks(appSettings.HealthChecksEndPoint, appSettings.DomainName);
app.ConfigureExceptionHandler();
app.UseHttpsRedirection();
app.UseAmbienteHeaderMiddleware();
app.UseOrigenHeaderMiddleware();
app.MapControllers();
app.Run();