using LA_LIGA_REKREATIVO.Client;
using LA_LIGA_REKREATIVO.Client.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddAntDesign();
builder.Services.AddScoped<Server>();
builder.Services.AddRadzenComponents();


await builder.Build().RunAsync();
