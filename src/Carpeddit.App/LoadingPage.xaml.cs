using Carpeddit.Api.Services;
using Carpeddit.App.Api.Helpers;
using Carpeddit.App.ViewModels;
using Carpeddit.App.Views;
using Carpeddit.Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Carpeddit.App
{
    public sealed partial class LoadingPage : Page
    {
        public LoadingPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            TitleBar.Loaded += (_, _1) => TitleBar.SetAsTitleBar();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            var settings = App.Services.GetService<SettingsViewModel>();
            Frame.RequestedTheme = settings.Theme;

            if (!await WebHelpers.CheckIsConnectedAsync())
            {
                Frame.Navigate(typeof(OfflinePage), null, new SuppressNavigationTransitionInfo());
                return;
            }

            /*if (!settings.SetupCompleted)
            {
                // TODO: Handle setup.
            }*/

            if (await IsValidSessionAsync())
            {
                Frame.Navigate(typeof(MainPage), null, new SuppressNavigationTransitionInfo());
                return;
            }

            Frame.Navigate(typeof(LoginPage), null, new SuppressNavigationTransitionInfo());
        }

        private async Task<bool> IsValidSessionAsync()
        {
            try
            {
                _ = await App.Services.GetService<IRedditService>().GetMeAsync();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);

                // Token must not be reused, sign out the user.
                await AccountHelper.Instance.SignOutAsync(false);
            }

            return false;
        }
    }
}
