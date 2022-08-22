using Carpeddit.App.Pages.Setup;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Carpeddit.App
{
    public sealed partial class FirstRunPage : Page
    {
        public FirstRunPage()
        {
            InitializeComponent();

            Navigate(true);
        }

        private void Navigate(bool suppressAnimation = false)
        {
            switch (App.SViewModel.SetupProgress)
            {
                case 0:
                default:
                    ContentFrame.Navigate(typeof(WelcomePage), null, suppressAnimation ? new SuppressNavigationTransitionInfo() : new EntranceNavigationTransitionInfo());
                    break;
                case 1:
                    if (App.CurrentAccount != null && App.CurrentAccount.LoggedIn)
                        ContentFrame.Navigate(typeof(LoginPage), null, suppressAnimation ? new SuppressNavigationTransitionInfo() : new EntranceNavigationTransitionInfo());
                    else
                        ContentFrame.Navigate(typeof(NotificationsPage), null, suppressAnimation ? new SuppressNavigationTransitionInfo() : new EntranceNavigationTransitionInfo());
                    break;
                case 2:
                    ContentFrame.Navigate(typeof(NotificationsPage), null, suppressAnimation ? new SuppressNavigationTransitionInfo() : new EntranceNavigationTransitionInfo());
                    break;
            }
        }

        private void OnBackButtonClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            App.SViewModel.SetupProgress--;
            Navigate();
        }

        private Visibility GreaterThanZero(int num)
            => num > 0 ? Visibility.Visible : Visibility.Collapsed;
    }
}
