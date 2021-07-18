using Autofac;
using Furventure.OfflineApplications.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Furventure.OfflineApplications
{
    public partial class App : Application
    {
        //private static IContainer Container { get; set; }

        public App()
        {
            InitializeComponent();

            registerServices();
            //ContainerStore.Container = Container;

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            Shell.Current.GoToAsync("//PlayPage");
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private void registerServices()
        {
            var builder = new ContainerBuilder();

        }
    }
}
