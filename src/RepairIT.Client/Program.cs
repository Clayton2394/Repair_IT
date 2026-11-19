using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RepairIT.Client;
using RepairIT.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Clients HTTP typés : les URLs des microservices viennent de wwwroot/appsettings.json
builder.Services.AddHttpClient<CatalogueApiClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Api:Catalogue"]!));

builder.Services.AddSingleton<NotificationService>();
builder.Services.AddScoped<TelechargementService>();

await builder.Build().RunAsync();
