using Carpeddit.App.ViewModels;
using Carpeddit.Common.Collections;
using Carpeddit.Models;
using Carpeddit.Models.Api;
using System;
using System.Linq;
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

        private BulkObservableCollection<PostViewModel> _posts = new();

        public SubredditInfoPage()
        {
            InitializeComponent();
        }

        private async void SubredditInfoPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadingInfoRing.IsActive = false;
            LoadingInfoRing.Visibility = Visibility.Collapsed;

            if (Subreddit.UserIsSubscriber ?? false)
                JoinButton.Content = "Leave";

            if (string.IsNullOrWhiteSpace(Subreddit.Title) || Subreddit.Title.Equals(Subreddit.DisplayNamePrefixed))
                _ = VisualStateManager.GoToState(this, "NoDisplayName", false);

            try
            {
                SubredditHeaderImg.Source = new BitmapImage(new(WebUtility.HtmlDecode(Subreddit.BannerBackgroundImage)));
            } catch (UriFormatException)
            {

            }

            PostLoadingProgressRing.IsActive = true;
            PostLoadingProgressRing.Visibility = Visibility.Visible;

            var posts = (await App.Client.GetSubredditPostsAsync(Subreddit.DisplayName, Api.Enums.SortMode.Hot)).Select(p => new PostViewModel()
            {
                Post = p
            });

            _posts.AddRange(posts);

            MainList.ItemsSource = _posts;

            PostLoadingProgressRing.IsActive = false;
            PostLoadingProgressRing.Visibility = Visibility.Collapsed;
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
