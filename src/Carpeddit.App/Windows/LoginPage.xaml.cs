using System.Web;
using System;
using Windows.UI.Xaml.Controls;
using Carpeddit.Common.Constants;
using Carpeddit.App.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;
using Windows.Security.Credentials;
using Carpeddit.App.ViewModels;
using System.Text.Json;

namespace Carpeddit.App
{
    public sealed partial class LoginPage : Page
    {
        private IRedditAuthService _authService = App.Services.GetService<IRedditAuthService>();
        private SettingsViewModel _settingsViewModel = App.Services.GetService<SettingsViewModel>();
        private IRedditService _service = App.Services.GetService<IRedditService>();

        public LoginPage()
        {
            InitializeComponent();

            LoginWebView.Source = new Uri($"https://www.reddit.com/api/v1/authorize?client_id={APIConstants.ClientId}&response_type=code&state=login&redirect_uri={APIConstants.RedirectUri}&duration=permanent&scope={string.Join(" ", Constants.Scopes)}");
            LoginWebView.NavigationStarting += LoginWebView_NavigationStarting;
        }

        private async void LoginWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri.AbsoluteUri.StartsWith(APIConstants.RedirectUri))
            {
                string oneTimeCode = HttpUtility.ParseQueryString(args.Uri.Query).Get("code");

                var token = Convert.ToBase64String(Encoding.ASCII.GetBytes(APIConstants.ClientId + ":" + APIConstants.ClientSecret));

                var dictionary = new Dictionary<string, string>()
                {
                    { "code", oneTimeCode },
                    { "grant_type", "authorization_code" },
                    { "redirect_uri", APIConstants.RedirectUri }
                };

                var authInfo = await _authService.GetAccessAsync(dictionary, token);

                var userInfo = await _service.GetCurrentlyAuthenticatedUserAsync(authInfo.AccessToken);

                var info = new
                {
                    accessToken = authInfo.AccessToken,
                    refreshToken = authInfo.RefreshToken
                };

                App.Valut.Add(new PasswordCredential("Reddit", userInfo.Name, JsonSerializer.Serialize(info)));

                var entry = App.Valut.Retrieve("Reddit", userInfo.Name);
                entry.RetrievePassword();

                Frame.Navigate(typeof(MainPage));

                // TODO: Check if setup is running instead
                // of going to the next setup page.
                // _settingsViewModel.SetupProgress++;
                // Frame.Navigate(typeof(NotificationsPage), new SlideNavigationTransitionInfo());
            }
        }
    }
}
