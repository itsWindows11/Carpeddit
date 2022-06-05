using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Pages
{
    public sealed partial class OfflinePage : Page
    {
        public OfflinePage()
        {
            InitializeComponent();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            bool networkAvailable = await Task.Run(() =>
            {
                var profile = NetworkInformation.GetInternetConnectionProfile();

                if (profile != null)
                {
                    return profile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.ConstrainedInternetAccess || profile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
                }

                return false;
            });

            if (networkAvailable)
            {
                Frame.Navigate((App.CurrentAccount != null && App.CurrentAccount.LoggedIn) ? typeof(MainPage) : typeof(LoginPage));
            }
        }
    }
}
