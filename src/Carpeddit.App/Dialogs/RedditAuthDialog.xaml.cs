using Carpeddit.Common.Constants;
using System;
using System.Web;
using Windows.UI.Xaml.Controls;
using Carpeddit.Api.Services;
using Carpeddit.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml;
using System.Text;

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

            var authInfo = await _authService.GetAccessAsync(oneTimeCode, Convert.ToBase64String(Encoding.ASCII.GetBytes(APIConstants.ClientId + ":" + APIConstants.ClientSecret)));

            (_authService as RedditAuthService).Data = authInfo;

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
