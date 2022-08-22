using Carpeddit.App.Controllers;
using Carpeddit.App.Models;
using Carpeddit.App.Other;
using Carpeddit.App.Pages.Setup;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Reddit;
using System;
using System.Web;
using Windows.ApplicationModel;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Carpeddit.App
{
    public sealed partial class LoginPage : Page
    {
        public static string Username { get; internal set; }

        public LoginPage()
        {
            InitializeComponent();

            LoginWebView.Source = new Uri("https://www.reddit.com/api/v1/authorize.compact?client_id=" + Constants.ClientId + "&response_type=code&state=login&redirect_uri=" + Constants.RedirectUri + "&duration=permanent&scope=creddits modcontributors modmail modconfig subscribe structuredstyles vote wikiedit mysubreddits submit modlog modposts modflair save modothers adsconversions read privatemessages report identity livemanage account modtraffic wikiread edit modwiki modself history flair");
            LoginWebView.NavigationStarting += LoginWebView_NavigationStarting;
        }

        private async void LoginWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri.AbsoluteUri.StartsWith(Constants.RedirectUri))
            {
                string oneTimeCode = HttpUtility.ParseQueryString(args.Uri.Query).Get("code");

                CustomAccountModel account = await AccountController.TryGetTokenInfoAsync(oneTimeCode);
                account.LoggedIn = true;

                App.RedditClient = new RedditClient(Constants.ClientId, account.RefreshToken, Constants.ClientSecret, account.AccessToken);

                await App.AccDBController.UpdateAsync(account);

                App.SViewModel.SetupProgress++;
                Frame.Navigate(typeof(NotificationsPage), new SlideNavigationTransitionInfo());
            }
        }
    }
}
