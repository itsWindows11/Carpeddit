using Carpeddit.Api.Enums;
using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using Carpeddit.App.Models;
using Carpeddit.App.ViewModels;
using Carpeddit.Common.Collections;
using Carpeddit.Common.Helpers;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using Windows.ApplicationModel.DataTransfer;
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
        private IRedditService service = App.Services.GetService<IRedditService>();

        private bool isLoadingMore;

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
                SubredditHeaderImg.Source = new BitmapImage(new(WebUtility.HtmlDecode(string.IsNullOrEmpty(Subreddit.BannerBackgroundImage) ? "ms-appx:///Assets/Dummy.png" : Subreddit.BannerBackgroundImage)));
            } catch (UriFormatException)
            {

            }

            PostLoadingProgressRing.IsActive = true;
            PostLoadingProgressRing.Visibility = Visibility.Visible;

            var posts = (await service.GetSubredditPostsAsync(Subreddit.DisplayName, SortMode.Hot, new(limit: 50))).Select(p => new PostViewModel()
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

            var posts = (await service.GetSubredditPostsAsync(Subreddit.DisplayName, SortMode.Hot, new(after: _posts.Last().Post.Name, limit: 50))).Select(p => new PostViewModel()
            {
                Post = p
            });

            _posts.AddRange(posts);

            FooterProgress.Visibility = Visibility.Collapsed;

            isLoadingMore = false;
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
                Subreddit = await service.GetSubredditInfoAsync(name);
                SubredditInfoPage_Loaded(null, null);
                Bindings.Update();
            }
        }

        [RelayCommand]
        private void UserClick(string name)
        {
            if (name == "[deleted]")
                return;

            Frame.Navigate(typeof(ProfilePage), name);
        }

        [RelayCommand]
        private void TitleClick(PostViewModel model)
            => ((Frame)Window.Current.Content).Navigate(typeof(PostDetailsPage), new PostDetailsNavigationInfo()
            {
                ShowFullPage = true,
                ItemData = model
            });

        private void OnCopyLinkFlyoutItemClick(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)e.OriginalSource).DataContext is not PostViewModel item)
                return;

            var package = new DataPackage()
            {
                RequestedOperation = DataPackageOperation.Copy,
            };

            package.SetText("https://www.reddit.com" + item.Post.Permalink);

            Clipboard.SetContent(package);
        }
    }
}
