using Carpeddit.App.Controllers;
using Carpeddit.App.Models;
using Carpeddit.App.ViewModels;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Reddit;
using Carpeddit.App.Other;
using System.Text;

namespace Carpeddit.App
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>

        public static SettingsViewModel SViewModel;

        public static HttpClient GlobalClient;

        public static AccountDatabaseController AccDBController;
        public static CustomAccountModel CurrentAccount;
        public static RedditClient RedditClient;

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        public async Task InitDb()
        {
            SViewModel = new SettingsViewModel();
            GlobalClient = new HttpClient();
            AccDBController = await AccountDatabaseController.Init();
            CurrentAccount = await AccDBController.GetAsync() ?? new CustomAccountModel();
            Debug.WriteLine($"I'm here! CurrentAccount is {(CurrentAccount != null ? "not null" : "null")}");
            if (CurrentAccount.RefreshToken != null)
            {
                RedditClient = new RedditClient(Constants.ClientId, CurrentAccount.RefreshToken, Constants.ClientSecret);
            }
        }

        public static int SubtractVotes(int upvotes, int downvotes)
        {
            return upvotes - downvotes;
        }

        public static string SubtractVotesAndFormat(int upvotes, int downvotes)
        {
            int ratio = upvotes - downvotes;

            if (ratio >= 100000000)
            {
                return (ratio / 1000000D).ToString("0.#M");
            }
            if (ratio >= 1000000)
            {
                return (ratio / 1000000D).ToString("0.##M");
            }
            if (ratio >= 100000)
            {
                return (ratio / 1000D).ToString("0.#k");
            }
            if (ratio >= 10000)
            {
                return (ratio / 1000D).ToString("0.##k");
            }

            return ratio.ToString("#,0");
        }

        public static string FormatNumber(long num)
        {
            if (num >= 100000000)
            {
                return (num / 1000000D).ToString("0.#M");
            }
            if (num >= 1000000)
            {
                return (num / 1000000D).ToString("0.##M");
            }
            if (num >= 100000)
            {
                return (num / 1000D).ToString("0.#k");
            }
            if (num >= 10000)
            {
                return (num / 1000D).ToString("0.##k");
            }

            return num.ToString("#,0");
        }

        public static string GetRelativeDate(DateTime time, bool approximate)
        {
            StringBuilder sb = new();

            string suffix = (time > DateTime.Now) ? " from now" : " ago";

            TimeSpan timeSpan = new(Math.Abs(DateTime.Now.Subtract(time).Ticks));

            if (timeSpan.Days > 0)
            {
                sb.AppendFormat("{0} {1}", timeSpan.Days,
                  (timeSpan.Days > 1) ? "days" : "day");
                if (approximate) return sb.ToString() + suffix;
            }
            if (timeSpan.Hours > 0)
            {
                sb.AppendFormat("{0}{1} {2}", (sb.Length > 0) ? ", " : string.Empty,
                  timeSpan.Hours, (timeSpan.Hours > 1) ? "hours" : "hour");
                if (approximate) return sb.ToString() + suffix;
            }
            if (timeSpan.Minutes > 0)
            {
                sb.AppendFormat("{0}{1} {2}", (sb.Length > 0) ? ", " : string.Empty,
                  timeSpan.Minutes, (timeSpan.Minutes > 1) ? "minutes" : "minute");
                if (approximate) return sb.ToString() + suffix;
            }
            if (timeSpan.Seconds > 0)
            {
                sb.AppendFormat("{0}{1} {2}", (sb.Length > 0) ? ", " : string.Empty,
                  timeSpan.Seconds, (timeSpan.Seconds > 1) ? "seconds" : "second");
                if (approximate) return sb.ToString() + suffix;
            }
            if (sb.Length == 0) return "right now";

            sb.Append(suffix);
            return sb.ToString();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            await InitApp(e);
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            /*if (args.Kind == ActivationKind.Protocol && args is ProtocolActivatedEventArgs args1)
            {
                // TODO: Handle URI activation
                // The received URI is eventArgs.Uri.AbsoluteUri
                // string param1 = HttpUtility.ParseQueryString(myUri.Query).Get("param1");
                Debug.WriteLine(args1.Uri.Query);
                
            }*/
            Window.Current.Activate();
        }


        private async Task InitApp(LaunchActivatedEventArgs e)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (Window.Current.Content is not Frame rootFrame)
            {
                await InitDb();

                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (!e.PrelaunchActivated)
            {
                Windows.ApplicationModel.Core.CoreApplication.EnablePrelaunch(true);
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate((CurrentAccount != null && CurrentAccount.LoggedIn) ? typeof(MainPage) : typeof(LoginPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
