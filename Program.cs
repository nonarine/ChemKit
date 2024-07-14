using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using ChemKit;
using ChemKit.Datasources;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsoleLogger());
ILogger logger = loggerFactory.CreateLogger("ChemKit");

builder.Services.AddSingleton(loggerFactory);
builder.Services.AddSingleton(logger);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMudServices();
builder.Services.AddDatasourceServices();

builder.Services.AddSingleton<LoadingBarService>();

var app = builder.Build();

await app.RunAsync();
