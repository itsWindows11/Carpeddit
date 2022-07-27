using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Carpeddit.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoadingPage : Page
    {
        public LoadingPage()
        {
            InitializeComponent();

            Loaded += LoadingPage_Loaded;
        }
        
        private async void LoadingPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Init database
            await App.InitDb();

            App.Logger.Information("[LoadingPage] Database initialized.");

            switch (App.SViewModel.Theme)
            {
                case 0:
                    (Window.Current.Content as Frame).RequestedTheme = ElementTheme.Light;
                    break;
                case 1:
                    (Window.Current.Content as Frame).RequestedTheme = ElementTheme.Dark;
                    break;
                case 2:
                    (Window.Current.Content as Frame).RequestedTheme = ElementTheme.Default;
                    break;
            }

            bool networkAvailable = await Task.Run(() =>
            {
                var profile = NetworkInformation.GetInternetConnectionProfile();

                if (profile != null)
                {
                    return profile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.ConstrainedInternetAccess || profile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
                }

                return false;
            });

            App.Logger.Information($"[LoadingPage] Network is {(networkAvailable ? "available" : "not available")}.");

            if (SystemInformation.Instance.IsFirstRun)
            {
                Frame.Navigate(typeof(FirstRunPage));
                return;
            }

            if (networkAvailable)
            {
                if (App.CurrentAccount != null && App.CurrentAccount.LoggedIn)
                {
                    Frame.Navigate(typeof(MainPage), null, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
                    App.Logger.Information("[MainPage] Loading frontpage...");
                }
                else
                {
                    Frame.Navigate(typeof(LoginPage), null, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
                }
            } else
            {
                Frame.Navigate(typeof(OfflinePage), null, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
            }

            App.Logger.Information("[LoadingPage] App successfully initialized.");
        }
    }
}
