using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Carpeddit.App.Pages.Setup
{
    public sealed partial class NotificationsPage : Page
    {
        public NotificationsPage()
        {
            InitializeComponent();
        }

        private void OnSecondaryActionClick(object sender, RoutedEventArgs e)
        {
            App.SViewModel.SetupProgress++;
            Frame.Navigate(typeof(SubscriptionPage), new SlideNavigationTransitionInfo());
        }
    }
}