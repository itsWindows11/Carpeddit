using Carpeddit.Api.Helpers;
using Carpeddit.App.ViewModels;
using Carpeddit.App.Views;
using Carpeddit.Common.Helpers;
using Carpeddit.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
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
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            var settings = App.Services.GetService<SettingsViewModel>();
            Frame.RequestedTheme = settings.Theme;

            var credentials = App.Valut.RetrieveAll();

            if (credentials.Any())
            {
                var credential = credentials.FirstOrDefault();
                credential.RetrievePassword();

                var json = JsonSerializer.Deserialize<PasswordToken>(credential.Password);

                App.Client = new(new()
                {
                    AccessToken = json.AccessToken,
                    RefreshToken = json.RefreshToken
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
            catch
            {
                foreach (var credential in App.Valut.FindAllByResource("Reddit"))
                    App.Valut.Remove(credential);
            }

            done:
            return false;
        }
    }
}
