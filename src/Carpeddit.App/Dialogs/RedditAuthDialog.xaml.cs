using Carpeddit.Common.Constants;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text;
using System.Web;
using Windows.Security.Credentials;
using Windows.UI.Xaml.Controls;
using Carpeddit.Api.Services;
using Carpeddit.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml;
using Carpeddit.Models;

namespace Carpeddit.App.Dialogs
{
    public sealed partial class RedditAuthDialog : ContentDialog
    {
        private IRedditAuthService _authService = App.Services.GetService<IRedditAuthService>();
        private SettingsViewModel _settingsViewModel = App.Services.GetService<SettingsViewModel>();
        private IRedditService _service = App.Services.GetService<IRedditService>();

        public bool IsCancelled { get; private set; }

        public RedditAuthDialog(Uri authUri)
        {
            InitializeComponent();

            LoginWebView.Source = authUri;
            LoginWebView.NavigationStarting += OnNavigationStarting;
        }

        private async void OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (!args.Uri.AbsoluteUri.StartsWith(APIConstants.RedirectUri))
                return;

            string oneTimeCode = HttpUtility.ParseQueryString(args.Uri.Query).Get("code");

            var token = Convert.ToBase64String(Encoding.ASCII.GetBytes(APIConstants.ClientId + ":" + APIConstants.ClientSecret));

            var dictionary = new Dictionary<string, string>()
            {
                { "code", oneTimeCode },
                { "grant_type", "authorization_code" },
                { "redirect_uri", APIConstants.RedirectUri }
            };

            var authInfo = await _authService.GetAccessAsync(dictionary, token);
            var userInfo = (await _service.GetCurrentlyAuthenticatedUserAsync(authInfo.AccessToken)).Content;

            var info = new PasswordToken
            {
                AccessToken = authInfo.AccessToken,
                RefreshToken = authInfo.RefreshToken
            };

            App.Valut.Add(new PasswordCredential("Reddit", userInfo.Name, JsonSerializer.Serialize(info)));

            await App.CacheRepository.UpsertAsync(new CachedUser()
            {
                Name = userInfo.Name,
                IconUrl = userInfo.IconImage,
                Created = userInfo.Created
            });

            App.Client = new(new()
            {
                AccessToken = info.AccessToken,
                RefreshToken = info.RefreshToken
            });

            // TODO: Check if setup is running instead
            // of going to the next setup page.
            // _settingsViewModel.SetupProgress++;
            // Frame.Navigate(typeof(NotificationsPage), new SlideNavigationTransitionInfo());

            Hide();
        }

        private void OnRefreshButtonClick(object sender, RoutedEventArgs e)
            => LoginWebView.Refresh();

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
            => LoginWebView.GoBack();

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            IsCancelled = true;
            Hide();
        }
    }
}
