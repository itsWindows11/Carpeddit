using Carpeddit.App.Collections;
using Carpeddit.App.Controllers;
using Carpeddit.App.Dialogs;
using Carpeddit.App.Helpers;
using Carpeddit.App.Models;
using Microsoft.Toolkit.Uwp;
using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Carpeddit.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        BulkConcurrentObservableCollection<PostViewModel> posts;
        BulkConcurrentObservableCollection<Subreddit> subreddits;

        public HomePage()
        {
            InitializeComponent();

            posts = new();
            subreddits = new();
            Loaded += Page_Loaded;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.Visibility = Visibility.Collapsed;
                FooterProgress.Visibility = Visibility.Visible;

                var posts1 = await Task.Run(() => GetPosts(after: posts[posts.Count - 1].Post.Fullname));

                posts.AddRange(posts1);

                button.Visibility = Visibility.Visible;
                FooterProgress.Visibility = Visibility.Collapsed;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Page_Loaded;

            UserImage.Source = new BitmapImage(new Uri(await Task.Run(() => AccountController.GetImageUrl(App.RedditClient.Account.Me.UserData)), UriKind.Absolute));

            LoadMoreButton.Visibility = Visibility.Collapsed;
            Progress.Visibility = Visibility.Visible;

            var posts1 = await Task.Run(() => GetPosts());
            var subreddits1 = await Task.Run(() => App.RedditClient.Account.MySubscribedSubreddits(limit: 100));

            posts.AddRange(posts1);
            subreddits.AddRange(subreddits1);

            SubredditsList.ItemsSource = subreddits1;
            MainList.ItemsSource = posts;

            Progress.Visibility = Visibility.Collapsed;
            LoadMoreButton.Visibility = Visibility.Visible;
            CreatePostPanel.Visibility = Visibility.Visible;

            try
            {
                SecondPageFrame.Navigate(typeof(SidebarPage), "home", new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
            }
            catch (COMException)
            {

            }
        }

        private IEnumerable<PostViewModel> GetPosts(string after = "", int limit = 100, string before = "")
        {
            List<Post> frontpage = App.RedditClient.GetFrontPage(limit: limit, after: after, before: before);
            List<PostViewModel> postViews = new();

            foreach (Post post in frontpage)
            {
                PostViewModel vm = new()
                {
                    Post = post,
                    Title = post.Title,
                    Description = post.GetDescription(),
                    Created = post.Created,
                    Subreddit = post.Subreddit,
                    Author = post.Author,
                    CommentsCount = post.Listing.NumComments
                };

                postViews.Add(vm);
            }

            return postViews;
        }

        private void Border_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(SubredditPage), (e.OriginalSource as FrameworkElement).DataContext);
        }

        private void MainList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SecondPageFrame.Navigate(typeof(PostDetailsPage), (e.AddedItems[0] as PostViewModel, false), new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
            }
            catch (COMException)
            {

            }

            var appViewTitleBar = ApplicationView.GetForCurrentView().TitleBar;

            appViewTitleBar.ButtonBackgroundColor = Colors.Transparent;
            appViewTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            Window.Current.SetTitleBar(MainPage.Current.AppTitleBar);

            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            MainPage.Current.AppTitleBar.Visibility = sender.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Update title bar control size as needed to account for system size changes.
            MainPage.Current.AppTitleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = MainPage.Current.AppTitleBar.Margin;
            MainPage.Current.AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private async void OnCreatePostButtonClick(object sender, RoutedEventArgs e)
        {
            _ = await new CreatePostDialog().ShowAsync();
        }
    }
}
