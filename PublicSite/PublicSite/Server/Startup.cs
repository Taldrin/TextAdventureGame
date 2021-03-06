using Autofac;
using Autofac.Extensions.DependencyInjection;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Core;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.DatabaseServices;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.GameLanguage;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Graph.Store;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Services.ImageStore;
using InterfurCreations.AdventureGames.Services.Interfaces;
using InterfurCreations.AdventureGames.SlackReporter;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;

namespace PublicSite.Server
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IContainer AutofacContainer { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            //services.AddHttpsRedirection(options =>
            //{
            //    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
            //    options.HttpsPort = 443;
            //});

            Log.LogMessage("Registering Autofac services...");
            //RegisterAutofac(services);

            services.AddHttpContextAccessor();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            //services.Configure<IISServerOptions>(options =>
            //{
            //    options.AutomaticAuthentication = true;
            //});

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IGameRetrieverService gameStore, IConfigurationService configService)
        {
            //app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
                Log.LogMessage("Starting up WebServer as Development");
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
                app.UseExceptionHandler("/error");
                Log.LogMessage("Starting up WebServer as Production");
            }

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
            Log.LogMessage("Finished configuration");

            string buildTypeName = null;

#if ReleaseLive
            buildTypeName = "PublicSiteLive";
#elif DebugAlpha
            buildTypeName = "PublicSiteAlpha";
#endif

            configService.SetConfig("TypeName", buildTypeName);

            ConfigSetting.DynamicApplicationName = buildTypeName;

            Log.LogMessage("Retrieving games");

            gameStore.ListGames();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            RegisterAutofac(builder);
        }

        public void RegisterAutofac(ContainerBuilder builder)
        {
            //var builder = new ContainerBuilder();
            builder.RegisterType<AppSettingsConfigurationService>().As<IConfigurationService>().InstancePerLifetimeScope();
            builder.RegisterType<DatabaseContextProvider>().As<IDatabaseContextProvider>().InstancePerLifetimeScope();
            builder.RegisterType<AccountController>().As<IAccountController>().InstancePerLifetimeScope();
            builder.RegisterType<PlayerDatabaseController>().As<IPlayerDatabaseController>().InstancePerLifetimeScope();
            builder.RegisterType<ConfigSettingsGoogleDriveAuthenticator>().As<IGoogleDriveAuthenticator>().InstancePerLifetimeScope();
            builder.RegisterType<GoogleDriveService>().As<IGoogleDriveService>().InstancePerLifetimeScope();
            builder.RegisterType<AccessTokenService>().As<IAccessTokenService>().InstancePerLifetimeScope();
            builder.RegisterType<TokenGenerator>().As<ITokenGenerator>().InstancePerLifetimeScope();
            builder.RegisterType<SlackReport>().As<IReporter>().InstancePerLifetimeScope();

            builder.RegisterType<ImagingService>().As<IImagingService>().SingleInstance();
            builder.RegisterType<AwsImageStore>().As<IImageStore>().InstancePerLifetimeScope();
            builder.RegisterType<LanguageToolSpellChecker>().As<ISpellChecker>().InstancePerLifetimeScope();
            builder.RegisterType<ImageBuildDataTracker>().InstancePerLifetimeScope();
            builder.RegisterType<ImageStoreCleanupTask>().InstancePerLifetimeScope();

            builder.RegisterType<GameExecutor>().As<IGameExecutor>().InstancePerLifetimeScope();
            builder.RegisterType<GameProcessor>().As<IGameProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<TextParsing>().As<ITextParsing>().InstancePerLifetimeScope();
            builder.RegisterType<GameDataService>().As<IGameDataService>().InstancePerLifetimeScope();
            builder.RegisterType<DatabaseDataStore>().As<IDataStore>().InstancePerLifetimeScope();
            builder.RegisterType<AccessService>().As<IAccessService>().InstancePerLifetimeScope();
            builder.RegisterType<TokenGenerator>().As<ITokenGenerator>().InstancePerLifetimeScope();
            builder.RegisterType<GameSaveService>().As<IGameSaveService>().InstancePerLifetimeScope();
            builder.RegisterType<ImagingService>().As<IImagingService>().InstancePerLifetimeScope();
            builder.RegisterType<ImageBuildDataTracker>().InstancePerLifetimeScope();

            builder.RegisterType<DrawStore>().As<IGameStore>().SingleInstance();
            builder.RegisterType<GameRetrieverService>().As<IGameRetrieverService>().SingleInstance();

            builder.RegisterAssemblyTypes(typeof(IMessageHandler).Assembly)
                .AssignableTo<IMessageHandler>()
                .AsImplementedInterfaces();

           // builder.Populate(services);

            //AutofacContainer = builder.Build();
        }
    }
}
