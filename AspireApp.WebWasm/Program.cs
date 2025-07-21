using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AspireApp.WebWasm;
using AspireApp.WebWasm.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddMudServices();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<LocationHubService>();

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = "http://localhost:8080/realms/AspireRealm";
    options.ProviderOptions.ClientId = "aspire-blazor-client";
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.RedirectUri = "http://localhost:5071/authentication/login-callback";
    options.ProviderOptions.PostLogoutRedirectUri = "/";
    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("email");
    options.UserOptions.RoleClaim = "roles"; // If using roles later
});

await builder.Build().RunAsync();
