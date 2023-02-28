using Carpeddit.Api.Enums;
using Carpeddit.Api.Helpers;
using Carpeddit.Api.Services;
using Carpeddit.App.Models;
using Carpeddit.App.ViewModels;
using Carpeddit.Common.Collections;
using Carpeddit.Common.Helpers;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Views
{
    public sealed partial class PopularPage : Page
    {
        private BulkObservableCollection<PostViewModel> _posts = new();
        private IRedditService service = App.Services.GetService<IRedditService>();

        private SortMode currentSort = SortMode.Hot;

        private bool isLoadingMore;

        public PopularPage()
        {
            InitializeComponent();
            Loaded += HomePage_Loaded;
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= HomePage_Loaded;

            var posts = (await service.GetSubredditPostsAsync("popular", currentSort, new(limit: 50))).Select(p => new PostViewModel()
            {
                Post = p
            });

            _posts.AddRange(posts);

            MainList.ItemsSource = _posts;

            HomeRing.IsActive = false;
            HomeRing.Visibility = Visibility.Collapsed;

            var scrollViewer = ListHelpers.GetScrollViewer(MainList);

            scrollViewer.ViewChanged += OnViewChanged;
        }

        private async void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;

            if (isLoadingMore || (scrollViewer.VerticalOffset > scrollViewer.ScrollableHeight - 36 && e.IsIntermediate))
                return;

            isLoadingMore = true;

            if (_posts.LastOrDefault() == null)
            {
                isLoadingMore = false;
                return;
            }

            FooterProgress.Visibility = Visibility.Visible;

            var posts = (await service.GetSubredditPostsAsync("popular", SortMode.Best, new(after: _posts.Last().Post.Name, limit: 50))).Select(p => new PostViewModel()
            {
                Post = p
            });

            _posts.AddRange(posts);

            FooterProgress.Visibility = Visibility.Collapsed;

            isLoadingMore = false;
        }

        [RelayCommand]
        private void SubredditClick(string subreddit)
            => Frame.Navigate(typeof(SubredditInfoPage), subreddit.Substring(2));

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

        [RelayCommand]
        private async Task SortSelectionChangedAsync()
        {
            MainList.ItemsSource = null;
            _posts.Clear();

            currentSort = StringToSortTypeConverter.ToSortMode((string)SortControl.SelectedContent);

            HomeRing.Visibility = Visibility.Visible;
            HomeRing.IsActive = true;

            var posts = (await service.GetSubredditPostsAsync("popular", currentSort, new(limit: 50))).Select(p => new PostViewModel()
            {
                Post = p
            });

            _posts.AddRange(posts);

            MainList.ItemsSource = _posts;

            HomeRing.IsActive = false;
            HomeRing.Visibility = Visibility.Collapsed;
        }
    }
}
