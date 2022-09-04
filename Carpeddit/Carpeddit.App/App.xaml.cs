using Carpeddit.App.Controllers;
using Carpeddit.App.Models;
using Carpeddit.App.ViewModels;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Reddit;
using Carpeddit.App.Other;
using Carpeddit.App.Pages;
using Windows.UI;
using System.IO;
using Windows.Storage;
using Serilog;
using Serilog.Core;
using Carpeddit.App.Helpers;
using Windows.ApplicationModel.Core;
using Reddit.Controllers.EventArgs;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Carpeddit.App
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
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

        public static Logger Logger;

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;

            UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            if (Logger != null)
            {
                Logger.Error(e.Exception, "An exception occurred.");
            }
        }

        public static Color GetColorFromHex(string hex)
        {
            if (hex != null)
            {
                hex = hex.Replace("#", string.Empty);

                try
                {
                    byte r = (byte)Convert.ToUInt32(hex.Substring(0, 2), 16);
                    byte g = (byte)Convert.ToUInt32(hex.Substring(2, 2), 16);
                    byte b = (byte)Convert.ToUInt32(hex.Substring(4, 2), 16);

                    return Color.FromArgb(255, r, g, b);
                }
                catch
                {

                }
            }
            
            
            return Current.RequestedTheme == ApplicationTheme.Light ? Color.FromArgb(255, 255, 255, 255) : Color.FromArgb(255, 0, 0, 0);
        }

        public static Color GetTextColorFromHex(string hex)
        {
            if (hex != null)
            {
                hex = hex.Replace("#", string.Empty);

                try
                {
                    byte r = (byte)Convert.ToUInt32(hex.Substring(0, 2), 16);
                    byte g = (byte)Convert.ToUInt32(hex.Substring(2, 2), 16);
                    byte b = (byte)Convert.ToUInt32(hex.Substring(4, 2), 16);

                    return Color.FromArgb(255, r, g, b);
                }
                catch
                {

                }
            }

            return Current.RequestedTheme == ApplicationTheme.Light ? Color.FromArgb(255, 0, 0, 0) : Color.FromArgb(255, 255, 255, 255);
        }

        public static int AddOne(int num)
        {
            return num + 1;
        }

        public static bool OppositeOf(bool ean)
        {
            return !ean;
        }

        public static async Task InitDb()
        {
            SViewModel ??= new SettingsViewModel();
            GlobalClient ??= new HttpClient();
            AccDBController ??= await AccountDatabaseController.Init();
            CurrentAccount ??= await AccDBController.GetAsync() ?? new CustomAccountModel();
            if (CurrentAccount.RefreshToken != null)
            {
                RedditClient ??= new RedditClient(Other.Constants.ClientId, CurrentAccount.RefreshToken, Other.Constants.ClientSecret);
            }
        }

        public static double Subtract(double a, double b)
        {
            return a - b;
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
            await InitApp();
        }


        private async Task InitApp(LaunchActivatedEventArgs e = null)
        {
            // Initialize logger
            Logger = new LoggerConfiguration()
                        .WriteTo
                        .File(Path.Combine(ApplicationData.Current.LocalFolder.Path, "log.txt"))
                        .CreateLogger();

            LoggingHelper.LogInfo("[App] Initialized logger.");

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (Window.Current.Content is not Frame rootFrame)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e != null)
                {
                    if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                    {
                        //TODO: Load state from previously suspended application
                    }
                }
                
                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e != null)
            {
                if (!e.PrelaunchActivated)
                {
                    CoreApplication.EnablePrelaunch(true);
                    if (rootFrame.Content == null)
                    {
                        // When the navigation stack isn't restored navigate to the first page,
                        // configuring the new page by passing required information as a navigation
                        // parameter

                        rootFrame.Navigate(typeof(LoadingPage), e.Arguments);
                    }
                    // Ensure the current window is active
                    Window.Current.Activate();
                }
            } else
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter

                    rootFrame.Navigate(typeof(LoadingPage));
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);

            if (RedditClient != null && CurrentAccount != null && CurrentAccount.LoggedIn)
            {
                RedditClient.Account.Messages.InboxUpdated += MailboxUpdated;
                _ = RedditClient.Account.Messages.MonitorInbox(5);
            }
        }

        public static void MailboxUpdated(object sender, MessagesUpdateEventArgs e)
        {
            var message = e.NewMessages[0];

            ToastContentBuilder builder = new();

            _ = builder.AddArgument("action", "viewMessage");
            _ = builder.AddText($"New message from u/{message.Author}!");
            _ = builder.AddText(message.Body.Length > 60 ? message.Body.Substring(0, 60) + "..." : message.Body);

            builder.Show();
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

            if (Logger != null)
            {
                Logger.Dispose();
            }
            
            deferral.Complete();
        }
    }
}
