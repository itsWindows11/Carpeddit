using Carpeddit.App.ViewModels;
using Carpeddit.App.ViewModels.Pages;
using CommunityToolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Views
{
    public sealed partial class PopularPage : Page
    {
        public PopularPageViewModel ViewModel { get; } = Ioc.Default.GetService<PopularPageViewModel>();

        public PopularPage()
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
