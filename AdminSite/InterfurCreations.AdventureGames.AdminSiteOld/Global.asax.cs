using Autofac;
using Autofac.Integration.Mvc;
using BotAdminSite.Controllers;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.DatabaseServices;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BotAdminSite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            RegisterAutofac();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));
        }

        private static IContainer Container { get; set; }

        public static void RegisterAutofac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterType<ConfigurationService>().As<IConfigurationService>().InstancePerLifetimeScope();
            builder.RegisterType<DatabaseContextProvider>().As<IDatabaseContextProvider>().InstancePerLifetimeScope();
            builder.RegisterType<AccountController>().As<IAccountController>().InstancePerLifetimeScope();
            builder.RegisterType<PlayerDatabaseController>().As<IPlayerDatabaseController>().InstancePerLifetimeScope();
            builder.RegisterType<ConsoleAppGoogleDriveAuthenticator>().As<IGoogleDriveAuthenticator>().InstancePerLifetimeScope();
            builder.RegisterType<GoogleDriveService>().As<IGoogleDriveService>().InstancePerLifetimeScope();

            Container = builder.Build();
        }
    }
}
