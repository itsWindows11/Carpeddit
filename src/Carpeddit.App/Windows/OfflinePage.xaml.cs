using Carpeddit.Common.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Views
{
    public sealed partial class OfflinePage : Page
    {
        public OfflinePage()
        {
            InitializeComponent();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (!await WebHelpers.CheckIsConnectedAsync())
                return;

            Frame.Navigate(typeof(LoadingPage));
        }
    }
}
