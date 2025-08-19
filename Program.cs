using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PersonalAccountant;
using PersonalAccountant.Services;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register our services
builder.Services.AddScoped<IFinanceService, LocalStorageFinanceService>();
// This line is updated to register the new service
builder.Services.AddScoped<ICurrencyService, ExchangeRateApiService>();

// This sets up the default HttpClient for the app
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// This is the crucial line to register the service we created
builder.Services.AddScoped<IFinanceService, LocalStorageFinanceService>();

// This line builds and runs the application
await builder.Build().RunAsync();