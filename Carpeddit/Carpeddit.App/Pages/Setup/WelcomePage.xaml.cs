using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Carpeddit.App.Pages.Setup
{
    public sealed partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private void OnContinueClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            App.SViewModel.SetupProgress++;

            if (App.CurrentAccount != null && App.CurrentAccount.LoggedIn)
                Frame.Navigate(typeof(NotificationsPage), new SlideNavigationTransitionInfo());
            else
                Frame.Navigate(typeof(LoginPage), new SlideNavigationTransitionInfo());
        }
    }
}