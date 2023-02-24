using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Views
{
    public sealed partial class OfflinePage : Page
    {
        public OfflinePage()
        {
            InitializeComponent();
            TitleBar.SetAsTitleBar();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
            => Frame.Navigate(typeof(LoadingPage));
    }
}
