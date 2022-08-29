using Carpeddit.App.Helpers;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Pages
{
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

            LoggingHelper.LogInfo("[LoadingPage] Database initialized.");

            _ = AppCenterHelper.StartAppCenterAsync();

            /*if (SystemInformation.Instance.IsAppUpdated)
            {
                
            }*/

            var tasks = BackgroundTaskRegistration.AllTasks;
            foreach (var task in tasks)
            {
                task.Value.Unregister(true);
            }

            BackgroundExecutionManager.RemoveAccess();

            var requestStatus = await BackgroundExecutionManager.RequestAccessAsync();

            if (requestStatus != BackgroundAccessStatus.AllowedSubjectToSystemPolicy && requestStatus != BackgroundAccessStatus.AlwaysAllowed)
            {
                System.Diagnostics.Debug.WriteLine("------ Cannot use background tasks.");
            } else
            {
                var builder = new BackgroundTaskBuilder
                {
                    Name = "Carpeddit - Notifications Background Task"
                };
                builder.SetTrigger(new TimeTrigger(15, false));

                var task = builder.Register();

                task.Completed += (s, e) =>
                {
                    _ = App.RedditClient.Account.Messages.MonitorInbox();
                    App.RedditClient.Account.Messages.InboxUpdated -= App.MailboxUpdated;
                };
            }

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

            LoggingHelper.LogInfo($"[LoadingPage] Network is {(networkAvailable ? "available" : "not available")}.");

            if (!App.SViewModel.SetupCompleted)
            {
                Frame.Navigate(typeof(FirstRunPage));
                return;
            }

            if (networkAvailable)
            {
                if (App.CurrentAccount != null && App.CurrentAccount.LoggedIn)
                {
                    Frame.Navigate(typeof(MainPage), null, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
                    LoggingHelper.LogInfo("[MainPage] Loading frontpage...");
                }
                else
                {
                    Frame.Navigate(typeof(LoginPage), null, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
                }
            } else
            {
                Frame.Navigate(typeof(OfflinePage), null, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
            }

            LoggingHelper.LogInfo("[LoadingPage] App successfully initialized.");
        }
    }
}
