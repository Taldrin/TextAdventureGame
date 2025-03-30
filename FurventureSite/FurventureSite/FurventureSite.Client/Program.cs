using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using FurventureSite.Client.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services
    .AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    }).AddBootstrap5Providers().AddFontAwesomeIcons();
builder.Services.AddScoped<MenuService>();
await builder.Build().RunAsync();
