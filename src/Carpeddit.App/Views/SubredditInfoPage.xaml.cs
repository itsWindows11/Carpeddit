using Carpeddit.Models.Api;
using System;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Carpeddit.App.Views
{
    public sealed partial class SubredditInfoPage : Page
    {
        private Subreddit Subreddit;

        public SubredditInfoPage()
        {
            InitializeComponent();
        }

        private void SubredditInfoPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadingInfoRing.Visibility = Visibility.Collapsed;

            if (Subreddit.UserIsSubscriber ?? false)
                JoinButton.Content = "Leave";

            if (string.IsNullOrWhiteSpace(Subreddit.HeaderTitle) || Subreddit.HeaderTitle.Equals(Subreddit.Name))
                _ = VisualStateManager.GoToState(this, "NoDisplayName", false);

            try
            {
                SubredditHeaderImg.Source = new BitmapImage(new(WebUtility.HtmlDecode(Subreddit.BannerBackgroundImage)));
            } catch (UriFormatException)
            {

            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Subreddit subreddit)
            {
                Subreddit = subreddit;
                Loaded += SubredditInfoPage_Loaded;
            }
            else if (e.Parameter is string name)
            {
                Subreddit = await App.Client.GetSubredditAsync(name);
                SubredditInfoPage_Loaded(null, null);
                Bindings.Update();
            }
        }
    }
}
