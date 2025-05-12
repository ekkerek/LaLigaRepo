using LA_LIGA_REKREATIVO.Client;
using LA_LIGA_REKREATIVO.Client.Server;
using LA_LIGA_REKREATIVO.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

string url = builder.Configuration["API_URL"];

// Add services
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<JwtAuthenticationStateProvider>());

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(url) });
builder.Services.AddScoped<Server>();
builder.Services.AddScoped<FileService>();

// Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// AntDesign (added once)
builder.Services.AddAntDesign();

// Build host
var host = builder.Build();

// Set culture
const string defaultCulture = "sr-Latn";
var js = host.Services.GetRequiredService<IJSRuntime>();
var result = await js.InvokeAsync<string>("blazorCulture.get");
var culture = CultureInfo.GetCultureInfo(defaultCulture);

// Save culture if not set
if (result == null)
{
    await js.InvokeVoidAsync("blazorCulture.set", defaultCulture);
}

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

// Run
await host.RunAsync();
