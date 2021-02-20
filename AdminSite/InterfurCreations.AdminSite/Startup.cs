using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using InterfurCreations.AdminSite.Core;
using InterfurCreations.AdminSite.Core.Interfaces;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.DatabaseServices;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Graph.Store;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace InterfurCreations.AdminSite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            ConfigSetting.DynamicApplicationName = "AdminSite";
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMicrosoftIdentityWebAppAuthentication(Configuration, "AzureAd");

            services.AddControllersWithViews().AddMvcOptions(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
              .RequireAuthenticatedUser()
              .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddMicrosoftIdentityUI();

            RegisterAutofac(services);

            return new AutofacServiceProvider(AutofacContainer);
        }

        public IContainer AutofacContainer { get; set; }

        public void RegisterAutofac(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<AppSettingsConfigurationService>().As<IConfigurationService>().InstancePerLifetimeScope();
            builder.RegisterType<DatabaseContextProvider>().As<IDatabaseContextProvider>().InstancePerLifetimeScope();
            builder.RegisterType<AccountController>().As<IAccountController>().InstancePerLifetimeScope();
            builder.RegisterType<PlayerDatabaseController>().As<IPlayerDatabaseController>().InstancePerLifetimeScope();
            builder.RegisterType<ConfigSettingsGoogleDriveAuthenticator>().As<IGoogleDriveAuthenticator>().InstancePerLifetimeScope();
            builder.RegisterType<GoogleDriveService>().As<IGoogleDriveService>().InstancePerLifetimeScope();
            builder.RegisterType<AccessTokenService>().As<IAccessTokenService>().InstancePerLifetimeScope();
            builder.RegisterType<TokenGenerator>().As<ITokenGenerator>().InstancePerLifetimeScope();
            builder.RegisterType<ActionResolver>().As<IActionResolver>().InstancePerLifetimeScope();
            builder.RegisterType<EmptyReporterService>().As<IReporter>().InstancePerLifetimeScope();
            builder.RegisterType<ActionResolver>().As<IActionResolver>().InstancePerLifetimeScope();
            builder.RegisterType<ReportsService>().As<IReportsService>().InstancePerLifetimeScope();

            builder.RegisterType<DrawStore>().As<IGameStore>().SingleInstance();
            builder.RegisterType<GameRetrieverService>().As<IGameRetrieverService>().SingleInstance();

            builder.Populate(services);

            AutofacContainer = builder.Build();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute("defalt", "{controller=Home}/{action=Index}/{id?}");

                //routes.MapRoute(
                //    name: "default",
                //    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
