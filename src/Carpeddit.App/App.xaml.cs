using Carpeddit.Api.Services;
using Carpeddit.Api.Watchers;
using Carpeddit.App.Services;
using Carpeddit.App.ViewModels;
using Carpeddit.App.ViewModels.Pages;
using Carpeddit.Repository;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static Carpeddit.Api.Watchers.MailboxWatcher;
using Carpeddit.Common.Constants;

namespace Carpeddit.App
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        public static IRepository CacheRepository { get; private set; }

        public static MailboxWatcher MailboxWatcher { get; set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            _ = ConfigureServices();

            if (!string.IsNullOrEmpty(Secrets.AppCenterSecret))
                AppCenter.Start(Secrets.AppCenterSecret, typeof(Analytics), typeof(Crashes));
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<IRedditAuthService, RedditAuthService>();
            services.AddSingleton<IRedditService, RedditService>();
            services.AddTransient<HomePageViewModel>();
            services.AddTransient<PopularPageViewModel>();
            services.AddTransient<SettingsPageViewModel>();
            services.AddTransient<SubredditInfoPageViewModel>();
            services.AddTransient<ProfilePageViewModel>();
            services.AddTransient<MailboxPageViewModel>();

            var provider = services.BuildServiceProvider();
            Ioc.Default.ConfigureServices(provider);

            return provider;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (Window.Current.Content is not Frame rootFrame)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new();
                rootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (!e.PrelaunchActivated)
            {
                CoreApplication.EnablePrelaunch(true);

                if (rootFrame.Content == null)
                    rootFrame.Navigate(typeof(LoadingPage), e.Arguments);
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);

            MailboxWatcher = await WatchMailboxAsync(Ioc.Default.GetService<IRedditService>(), TimeSpan.FromMinutes(1));
            MailboxWatcher.MailboxUpdated += OnMailboxUpdated;
        }

        private void OnMailboxUpdated(object sender, MailboxUpdateEventArgs e)
        {
            var message = e.Messages.FirstOrDefault();

            if (message == null) return;

            var builder = new ToastContentBuilder();

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
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load page " + e.SourcePageType.FullName);
        }
    }
}
