using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;

namespace InterfurCreations.AdminSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                            .ConfigureAppConfiguration(config =>
                            {
                                var connection = Environment.GetEnvironmentVariable("ConnectionString");
                                config.AddAzureAppConfiguration(connection);
                            })
                .ConfigureServices(services => services.AddAutofac())
                .UseUrls("http://localhost:5200")
                .UseUrls("https://localhost:5201")
                .UseStartup<Startup>();
        }
    }
}
