using InterfurCreations.AdventureGames.Kik;
using Microsoft.AspNet.WebHooks;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;

[assembly: OwinStartup(typeof(InterfurCreations.AdventureGames.Webhook.Startup))]
namespace InterfurCreations.AdventureGames.Webhook
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

            WebHookAssemblyResolver assemblyResolver = new WebHookAssemblyResolver();
            config.Services.Replace(typeof(IAssembliesResolver), assemblyResolver);

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);

            config.InitializeReceiveCustomWebHooks();
            config.InitializeReceiveGenericJsonWebHooks();
        }
    }
}