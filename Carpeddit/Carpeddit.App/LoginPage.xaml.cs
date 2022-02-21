using Carpeddit.App.Controllers;
using Carpeddit.App.Models;
using Carpeddit.App.Other;
using Newtonsoft.Json.Linq;
using Reddit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Web;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Carpeddit.App
{
    public sealed partial class LoginPage : Page
    {
        private bool _isOnAuthPage;

        public static string Username { get; internal set; }

        public LoginPage()
        {
            InitializeComponent();

            var appViewTitleBar = ApplicationView.GetForCurrentView().TitleBar;

            appViewTitleBar.ButtonBackgroundColor = Colors.Transparent;
            appViewTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            Window.Current.SetTitleBar(AppTitleBar);

            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            LoginWebView.Source = new Uri("https://www.reddit.com/api/v1/authorize?client_id=" + Constants.ClientId + "&response_type=code&state=login&redirect_uri=" + Constants.RedirectUri + "&duration=permanent&scope=creddits modcontributors modmail modconfig subscribe structuredstyles vote wikiedit mysubreddits submit modlog modposts modflair save modothers adsconversions read privatemessages report identity livemanage account modtraffic wikiread edit modwiki modself history flair");
            LoginWebView.NavigationStarting += LoginWebView_NavigationStarting;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            AppTitleBar.Visibility = sender.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private async void LoginWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri.AbsoluteUri.Contains(Constants.RedirectUri))
            {
                string oneTimeCode = HttpUtility.ParseQueryString(args.Uri.Query).Get("code");

                AuthViewModel tokenInfo = await AccountController.TryGetTokenInfoAsync(oneTimeCode);

                CustomAccountModel account = new();

                account.AccessToken = tokenInfo?.AccessToken;
                account.RefreshToken = tokenInfo?.RefreshToken;
                account.Scope = tokenInfo?.Scope;
                account.TokenExpiresIn = tokenInfo?.ExpiresIn;
                account.LoggedIn = true;
                App.RedditClient = new RedditClient(Constants.ClientId, account.RefreshToken, Constants.ClientSecret);

                await App.AccDBController.UpdateAsync(account);

                if (Window.Current.Content is Frame rootFrame)
                {
                    rootFrame.Navigate(typeof(MainPage));
                }
            }
        }
    }
}
