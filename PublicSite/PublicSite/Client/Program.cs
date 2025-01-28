using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Blazorise;
using Blazorise.Bootstrap;
using PublicSite.Client.Components;

namespace PublicSite.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            var originalUri = new Uri(builder.HostEnvironment.BaseAddress);

            var uriBuilder = new UriBuilder(originalUri)
            {
                Host = "api." + originalUri.Host
            };

            builder.Services
                .AddTransient(sp => new HttpClient { BaseAddress = uriBuilder.Uri })
                .AddBlazorise(options =>
                {

                }).AddBootstrapProviders()
                .AddSingleton<IMenuService, MenuService>();

            var host = builder.Build();

            host.Services.UseBootstrapProviders();
            
            await host.RunAsync();
        }
    }
}
