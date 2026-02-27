using favapp;
using favapp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

/*builder.Services.AddOidcAuthentication(options =>
{
    // Configure your authentication provider options here.
    // For more information, see https://aka.ms/blazor-standalone-auth
    builder.Configuration.Bind("Local", options.ProviderOptions);
});*/

// Active les fonctionnalités <AuthorizeView> et [Authorize] sans exiger de serveur externe
builder.Services.AddAuthorizationCore();

// AJOUTE JUSTE CA :
builder.Services.AddScoped<AuthenticationStateProvider, favapp.Services.CustomAuthStateProvider>();

builder.Services.AddScoped<ITmdbService, TmdbService>(); //  Scoped veut dire que c'est une seule et unique instance de l'objet contrairement à Transient qui lui crée differente instance
                                                         // Singleton est partagé à toute les requetes
builder.Services.AddScoped<favapp.Services.FavoriteService>();


await builder.Build().RunAsync();
