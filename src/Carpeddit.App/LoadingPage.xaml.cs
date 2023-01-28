using Carpeddit.Api.Helpers;
using Carpeddit.App.Api.Helpers;
using Carpeddit.App.ViewModels;
using Carpeddit.App.Views;
using Carpeddit.Common.Helpers;
using Carpeddit.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
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

            var currentInfo = AccountHelper.GetCurrentInfo();

            if (currentInfo != null)
            {
                App.Client = new(new()
                {
                    AccessToken = currentInfo.AccessToken,
                    RefreshToken = currentInfo.RefreshToken
                });
            }

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
            if (App.Client == null)
                goto done;

            try
            {
                _ = await App.Client.Account.GetMeAsync();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);

                try
                {
                    await TokenHelper.RefreshTokenAsync(App.Client.Info.RefreshToken);
                }
                catch
                {
                    // All tokens seem to be invalid, sign out the user.
                    await AccountHelper.SignOutAsync(false);
                }
            }

        done:
            return false;
        }
    }
}
