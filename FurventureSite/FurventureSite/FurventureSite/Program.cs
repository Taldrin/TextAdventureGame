using Autofac;
using Autofac.Extensions.DependencyInjection;
using FurventureSite;
using FurventureSite.Client.Components;
using FurventureSite.Client.Pages;
using FurventureSite.Client.Components;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Graph.Store;
using InterfurCreations.AdventureGames.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        var connection = Environment.GetEnvironmentVariable("ConnectionString");
        config.AddAzureAppConfiguration(connection);
    })
    .UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    new ConfigureStartup(builder.Configuration).ConfigureContainer(containerBuilder);
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddControllers();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    }).AddBootstrap5Providers().AddFontAwesomeIcons();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCookiePolicy();
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode();

app.MapControllers();

#if !DEBUG
    var configService = app.Services.GetService<IConfigurationService>();
    var gameRetService = app.Services.GetService<IGameRetrieverService>();
    
    configService.SetConfig("TypeName", ConfigSetting.DynamicApplicationName);
    Log.LogMessage("Retrieving games");
    gameRetService.ListGames();
#endif

app.Run();