﻿using System;
using Windows.UI.Xaml.Controls;
using Carpeddit.Common.Constants;
using Windows.UI.Xaml;
using Carpeddit.App.Dialogs;

namespace Carpeddit.App
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnEmailOptionClick(object sender, RoutedEventArgs e)
        {
            var authorizeUri = new Uri($"https://www.reddit.com/api/v1/authorize?client_id={APIConstants.ClientId}&response_type=code&state=login&redirect_uri={APIConstants.RedirectUri}&duration=permanent&scope={string.Join(" ", Constants.Scopes)}");

            var dialog = new RedditAuthDialog(authorizeUri);
            _ = await dialog.ShowAsync();

            if (!dialog.IsCancelled)
                Frame.Navigate(typeof(MainPage));
        }

        private async void OnRegisterButtonClick(object sender, RoutedEventArgs e)
        {
            var authorizeUri = new Uri($"https://www.reddit.com/account/register/?dest=https://www.reddit.com/api/v1/authorize?client_id={APIConstants.ClientId}&response_type=code&state=login&redirect_uri={APIConstants.RedirectUri}&duration=permanent&scope={string.Join(" ", Constants.Scopes)}");

            var dialog = new RedditAuthDialog(authorizeUri);
            _ = await dialog.ShowAsync();

            if (!dialog.IsCancelled)
                Frame.Navigate(typeof(MainPage));
        }
    }
}
