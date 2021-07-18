using Furventure.OfflineApplications.ViewModels;
using Furventure.OfflineApplications.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Furventure.OfflineApplications
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(PlayPage), typeof(PlayPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));

        }
    }
}
