using Carpeddit.App.ViewModels;
using Carpeddit.App.ViewModels.Pages;
using CommunityToolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Views
{
    public sealed partial class HomePage : Page
    {
        public HomePageViewModel ViewModel { get; } = Ioc.Default.GetService<HomePageViewModel>();

        public HomePage()
        {
            InitializeComponent();
        }

        private void OnCopyLinkFlyoutItemClick(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)e.OriginalSource).DataContext is not PostViewModel item)
                return;

            ViewModel.CopyLinkCommand?.Execute(item);
        }
    }
}
