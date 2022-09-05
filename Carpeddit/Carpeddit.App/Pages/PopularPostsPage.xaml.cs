using Carpeddit.App.Collections;
using Carpeddit.App.Helpers;
using Carpeddit.App.Models;
using Carpeddit.App.ViewModels;
using Carpeddit.Common.Helpers;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Pages
{
    public sealed partial class PopularPostsPage : Page
    {
        BulkConcurrentObservableCollection<PostViewModel> posts = new();

        bool isLoadingMore;

        public PopularPostsPage()
        {
            InitializeComponent();

            Loaded += Page_Loaded;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.Visibility = Visibility.Collapsed;
                FooterProgress.Visibility = Visibility.Visible;

                await foreach (var post in PostHelpers.GetPopularAsync(after: posts.Last().Post.Fullname))
                {
                    posts.Add(post);
                }

                button.Visibility = Visibility.Visible;
                FooterProgress.Visibility = Visibility.Collapsed;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Page_Loaded;

            Progress.Visibility = Visibility.Visible;

            await foreach (var post in PostHelpers.GetPopularAsync())
            {
                posts.Add(post);
            }

            MainList.ItemsSource = posts;

            Progress.Visibility = Visibility.Collapsed;

            var scrollViewer = ListHelpers.GetScrollViewer(MainList);

            scrollViewer.ViewChanged += async (s, e) =>
            {
                if (!isLoadingMore && scrollViewer.VerticalOffset <= scrollViewer.ScrollableHeight - 50)
                {
                    isLoadingMore = true;

                    FooterProgress.Visibility = Visibility.Visible;

                    await foreach (var post in PostHelpers.GetPopularAsync(after: posts.Last().Post.Fullname, limit: 30))
                    {
                        posts.Add(post);
                    }

                    FooterProgress.Visibility = Visibility.Collapsed;

                    isLoadingMore = false;
                }
            };

            try
            {
                SecondPageFrame.Navigate(typeof(SidebarPage), "popular", new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
            }
            catch (COMException)
            {

            }
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
    }
}
