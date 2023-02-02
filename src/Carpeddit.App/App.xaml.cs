using Carpeddit.Api.Helpers;
using Carpeddit.Api.Services;
using Carpeddit.App.Services;
using Carpeddit.App.ViewModels;
using Carpeddit.App.ViewModels.Pages;
using Carpeddit.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Carpeddit.App
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        public static IRepository CacheRepository { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Services = ConfigureServices();

            // Initialize the helpers.
            _ = new AccountHelper(Services.GetService<IRedditAuthService>(), Services.GetService<IRedditService>());
            _ = new TokenHelper(Services.GetService<IRedditAuthService>());
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<IRedditAuthService, RedditAuthService>();
            services.AddSingleton<IRedditService, RedditService>();
            services.AddTransient<SettingsPageViewModel>();

            return services.BuildServiceProvider();
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
