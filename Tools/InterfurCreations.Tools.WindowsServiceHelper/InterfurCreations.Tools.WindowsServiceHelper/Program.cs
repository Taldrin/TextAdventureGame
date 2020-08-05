using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InterfurCreations.Tools.WindowsServiceHelper
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Adventure game windows service helper");
            while (true)
            {
                Console.Write("Input directory of service to install: ");
                var path = Console.ReadLine();

                if (!Directory.Exists(path)) continue;

                var files = Directory.GetFiles(path);
                var file = files.FirstOrDefault(a => Path.GetFileName(a) == "InterfurCreations.AdventureGames.WorkerService.exe");

                if (string.IsNullOrEmpty(file))
                {
                    Console.WriteLine("No worker service exe found");
                    files.ToList().ForEach(a => Console.WriteLine(a));
                    continue;
                }

                Console.Write("Service name: ");

                var name = Console.ReadLine();

                Install(name, file);
            }
        }

        static void Install(string serviceName, string exePath)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            var serviceController = new ServiceController(serviceName);

            var domain = Assembly.GetExecutingAssembly().Location;

            ServiceController[] services = ServiceController.GetServices();
            var existingService = services.FirstOrDefault(s => s.ServiceName == serviceName);

            if (Environment.UserInteractive)
            {
                if (existingService != null)
                {
                    if (serviceController.Status == ServiceControllerStatus.Running)
                        serviceController.Stop();
                    Console.WriteLine("Service already found for bot: " + serviceName);
                    Console.Write("Reinstall? (Y/N)");
                    var shouldReinstall = Console.ReadLine();
                    if (shouldReinstall.ToLower() == "n") return;
                    Console.WriteLine("Uninstalling existing service...");

                    RunCommand($"/C SC DELETE {serviceName}");
                }
                else
                {
                    Console.Write("No service found. Install as new? (Y/N)");
                    var shouldInstallNew = Console.ReadLine();
                    if (shouldInstallNew.ToLower() == "n") return;
                }

                RunCommand($"/C SC CREATE {serviceName} binpath= \"{exePath}\" start= delayed-auto");
                RunCommand($"/C SC FAILURE {serviceName} reset= 0 actions= restart/8000/restart/15000/restart/30000");

                Console.Write("Service installed. Waiting 2 seconds before attempting to start...");
                Thread.Sleep(2000);
                Console.Write("Starting service");
                RunCommand($"/C SC START {serviceName}");
                Console.Write("Finished starting service");
            }
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            Console.Write(ex.Message + " " + ex.StackTrace);
            File.AppendAllText(@"C:\Temp\error.txt", ((Exception)e.ExceptionObject).Message + ((Exception)e.ExceptionObject).InnerException.Message);
        }

        private static void RunCommand(string command)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo("cmd.exe", command)
            {
                CreateNoWindow = false,
                WorkingDirectory = "C:\\",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Verb = "runas"
            };
            p.Start();
            Console.WriteLine(p.StandardOutput.ReadToEnd());
            Console.WriteLine(p.StandardError.ReadToEnd());
            p.WaitForExit();
            p.Close();
        }
    }
}
