using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.UI.Xaml.Controls;

using Test.ViewModels;

namespace Test.Views
{
    // TODO WTS: Change the grid as appropriate to your app, adjust the column definitions on DataGridPage.xaml.
    // For more details see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid
    public sealed partial class PlayPage : Page
    {
        public PlayViewModel ViewModel { get; }

        public PlayPage()
        {
            ViewModel = Ioc.Default.GetService<PlayViewModel>();
            InitializeComponent();
        }
    }
}
