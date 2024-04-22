using LA_LIGA_REKREATIVO.Client;
using LA_LIGA_REKREATIVO.Client.Server;
using LA_LIGA_REKREATIVO.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

string url = builder.Configuration["API_URL"];

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(url) });
builder.Services.AddAntDesign();
builder.Services.AddScoped<Server>();
builder.Services.AddScoped<FileService>();


await builder.Build().RunAsync();

