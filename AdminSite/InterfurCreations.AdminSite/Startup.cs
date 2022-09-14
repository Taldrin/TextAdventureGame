using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.SqlServer;
using InterfurCreations.AdminSite.Core;
using InterfurCreations.AdminSite.Core.Interfaces;
using InterfurCreations.AdminSite.BackgroundTasks.Tasks;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.DatabaseServices;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Graph.Store;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Services.ImageStore;
using InterfurCreations.AdventureGames.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using InterfurCreations.AdventureGames.GameTesting;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Core;
using InterfurCreations.AdventureGames.GameLanguage;
using InterfurCreations.AdminSite.BackgroundTasks;

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



            var config = new AppSettingsConfigurationService(Configuration);
            //var connectionString = "Server=localhost;Database=AdventureBot;Trusted_Connection=True";
            var connectionString = config.GetConfig("DatabaseConnectionString");
            services.AddHangfire(config => config.UseSqlServerStorage(connectionString, new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            }));

            services.AddHangfireServer();

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
            builder.RegisterType<GoogleDriveService>().As<IGoogleDriveService>().InstancePerLifetimeScope();
            builder.RegisterType<AccessTokenService>().As<IAccessTokenService>().InstancePerLifetimeScope();
            builder.RegisterType<TokenGenerator>().As<ITokenGenerator>().InstancePerLifetimeScope();
            builder.RegisterType<ActionResolver>().As<IActionResolver>().InstancePerLifetimeScope();
            builder.RegisterType<EmptyReporterService>().As<IReporter>().InstancePerLifetimeScope();
            builder.RegisterType<ActionResolver>().As<IActionResolver>().InstancePerLifetimeScope();
            builder.RegisterType<ReportsService>().As<IReportsService>().InstancePerLifetimeScope();
            builder.RegisterType<StatisticsService>().As<IStatisticsService>().InstancePerLifetimeScope();
            builder.RegisterType<BackupService>().InstancePerLifetimeScope();
            builder.RegisterType<DatabaseAdminFunctions>().InstancePerLifetimeScope();
            builder.RegisterType<GoogleDriveService>().As<IGoogleDriveService>().InstancePerLifetimeScope();

            #region GameTesting
            builder.RegisterType<GameProcessor>().As<IGameProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<GameTestDataProvider>().As<IGameTestDataProvider>().InstancePerLifetimeScope();
            builder.RegisterType<GameRetrieverService>().As<IGameRetrieverService>().SingleInstance();
            builder.RegisterType<DrawGameTestExecutor>().As<IGameTestExecutor>().InstancePerLifetimeScope();
            builder.RegisterType<LanguageToolSpellChecker>().As<ISpellChecker>().InstancePerLifetimeScope();
            builder.RegisterType<TextParsing>().As<ITextParsing>().InstancePerLifetimeScope();
            builder.RegisterType<GameDataService>().As<IGameDataService>().InstancePerLifetimeScope();
            builder.RegisterType<GameProcessor>().As<IGameProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<DrawStore>().As<IGameStore>().SingleInstance();
            builder.RegisterType<ConfigSettingsGoogleDriveReadWriteAuthenticator>().As<IGoogleDriveAuthenticator>().SingleInstance();
            builder.RegisterType<GoogleDriveService>().As<IGoogleDriveService>().SingleInstance();
            builder.RegisterType<DrawGameTestExecutor>().As<DrawGameTestExecutor>().SingleInstance();
            builder.RegisterType<LanguageToolSpellChecker>().As<ISpellChecker>().SingleInstance();
            builder.RegisterType<EmptyImagineService>().As<IImagingService>().SingleInstance();
            builder.RegisterType<ImageBuildDataTracker>().As<ImageBuildDataTracker>().SingleInstance();
            builder.RegisterType<GameTestingReportCompiler>().InstancePerLifetimeScope();
            #endregion

            builder.RegisterType<AwsImageStore>().As<IImageStore>().SingleInstance();
            builder.RegisterType<DrawStore>().As<IGameStore>().SingleInstance();

            builder.RegisterType<AchievementStatisticsBuildTask>().InstancePerLifetimeScope();
            builder.RegisterType<BackupTask>().InstancePerLifetimeScope();
            builder.RegisterType<GameTestingTask>().InstancePerLifetimeScope();

            builder.Populate(services);

            AutofacContainer = builder.Build();
            GlobalConfiguration.Configuration.UseAutofacActivator(AutofacContainer);
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

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new [] { new HangfireAuthorizationFilter() }
            });
            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute("defalt", "{controller=Home}/{action=Index}/{id?}");
                routes.MapHangfireDashboard();
            });

            SetupHangfireJobs();
        }

        public void SetupHangfireJobs()
        {
            RecurringJob.AddOrUpdate<AchievementStatisticsBuildTask>(a => a.Run(), Cron.MinuteInterval(10));
            RecurringJob.AddOrUpdate<GameTestingTask>(a => a.Run(), Cron.MinuteInterval(12));
            RecurringJob.AddOrUpdate<ImageStoreCleanupTask>(a => a.ClearImages(), Cron.HourInterval(2));
            RecurringJob.AddOrUpdate<GamesByPlayerCountStatisticsBuildTask>(a => a.Run(), Cron.HourInterval(12));
            RecurringJob.AddOrUpdate<BackupTask>(a => a.Run(), Cron.DayInterval(1));
        }
    }
}
