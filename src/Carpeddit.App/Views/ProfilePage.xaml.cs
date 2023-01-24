using Carpeddit.Common.Helpers;
using Carpeddit.Models;
using Carpeddit.Models.Api;
using System.Net;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Carpeddit.Common.Collections;
using System.Linq;
using Carpeddit.App.ViewModels;

namespace Carpeddit.App.Views
{
    public sealed partial class ProfilePage : Page
    {
        private User _user;
        private BulkObservableCollection<PostViewModel> _posts = new();
        private bool isLoadingMore;

        public ProfilePage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is User user)
                _user = user;
            else
                _user = await App.Client.Account.GetMeAsync();

            base.OnNavigatedTo(e);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadingInfoRing.IsActive = false;
            LoadingInfoRing.Visibility = Visibility.Collapsed;

            if (_user.Subreddit.UserIsSubscriber)
                JoinButton.Content = "Unfollow";

            if (string.IsNullOrWhiteSpace(_user.Subreddit.Title) || _user.Subreddit.DisplayName.Equals($"u_{_user.Name}"))
                _ = VisualStateManager.GoToState(this, "NoDisplayName", false);

            try
            {
                SubredditHeaderImg.Source = new BitmapImage(new(WebUtility.HtmlDecode(_user.Subreddit.BannerImage)));
            }
            catch (UriFormatException)
            {

            }

            PostLoadingProgressRing.IsActive = true;
            PostLoadingProgressRing.Visibility = Visibility.Visible;

            var posts = (await App.Client.GetUserPostsAsync(_user.Name, new(limit: 50))).Select(p => new PostViewModel()
            {
                Post = p
            });

            _posts.AddRange(posts);

            MainList.ItemsSource = _posts;

            PostLoadingProgressRing.IsActive = false;
            PostLoadingProgressRing.Visibility = Visibility.Collapsed;

            var scrollViewer = ListHelpers.GetScrollViewer(MainList);

            scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;

            if (isLoadingMore || scrollViewer.VerticalOffset > scrollViewer.ScrollableHeight - 50)
                return;

            isLoadingMore = true;

            FooterProgress.Visibility = Visibility.Visible;

            var posts = (await App.Client.GetUserPostsAsync(_user.Name, new(after: _posts.Last().Post.Name, limit: 50))).Select(p => new PostViewModel()
            {
                Post = p
            });

            _posts.AddRange(posts);

            FooterProgress.Visibility = Visibility.Collapsed;

            isLoadingMore = false;
        }
    }
}
