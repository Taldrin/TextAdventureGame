using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Services;
using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            var type = new ConfigurationService().GetConfig("TypeName");

            Log.LogMessage("Beginning bot: " + type);

            var serviceController = new ServiceController(type);

            ServiceController[] services = ServiceController.GetServices();
            var existingService = services.FirstOrDefault(s => s.ServiceName == type);

            if (Environment.UserInteractive)
            {
                string parameter = string.Concat(args);

                if (existingService != null)
                {
                    if(serviceController.Status == ServiceControllerStatus.Running)
                        serviceController.Stop();
                    Log.LogMessage("Service already found for bot: " + type);
                    Log.LogMessage("Uninstalling existing service...");

                    ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                }
                Log.LogMessage("Installing new service");
                ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                Log.LogMessage("Service installed. Waiting 2 seconds before attempting to start...");
                Thread.Sleep(2000);
                Log.LogMessage("Starting service");
                serviceController.Start();
            }
            else
            {
                ServiceBase.Run(new AdventureBot());
            }
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            Log.LogMessage(ex.Message + " " + ex.StackTrace);
            File.AppendAllText(@"C:\Temp\error.txt", ((Exception)e.ExceptionObject).Message + ((Exception)e.ExceptionObject).InnerException.Message);
        }
    }
}
