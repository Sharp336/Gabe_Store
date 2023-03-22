global using Blazored.LocalStorage;
global using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;

using Gabe_Store.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
GoodsProvider _goodsProvider = new();
builder.Services.AddSingleton<GoodsProvider>(_goodsProvider);
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddMudServices();



await builder.Build().RunAsync();
