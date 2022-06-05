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

            LoginWebView.Source = new Uri("https://www.reddit.com/api/v1/authorize?client_id=" + Constants.ClientId + "&response_type=code&state=login&redirect_uri=" + Constants.RedirectUri + "&duration=permanent&scope=creddits modcontributors modmail modconfig subscribe structuredstyles vote wikiedit mysubreddits submit modlog modposts modflair save modothers adsconversions read privatemessages report identity livemanage account modtraffic wikiread edit modwiki modself history flair");
            LoginWebView.NavigationStarting += LoginWebView_NavigationStarting;
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
