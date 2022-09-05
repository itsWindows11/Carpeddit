using Carpeddit.App.Collections;
using Carpeddit.App.Controllers;
using Carpeddit.App.Dialogs;
using Carpeddit.App.Helpers;
using Carpeddit.App.Models;
using Carpeddit.App.ViewModels;
using Carpeddit.Common.Helpers;
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

namespace Carpeddit.App.Pages
{
    public sealed partial class HomePage : Page
    {
        BulkConcurrentObservableCollection<PostViewModel> posts;

        bool isLoadingMore;

        public HomePage()
        {
            InitializeComponent();

            posts = new();
            Loaded += Page_Loaded;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Page_Loaded;

            Progress.Visibility = Visibility.Visible;

            await foreach (var post in PostHelpers.GetFrontpageAsync())
            {
                posts.Add(post);
            }

            MainList.ItemsSource = posts;
            Progress.Visibility = Visibility.Collapsed;
            CreatePostPanel.Visibility = Visibility.Visible;

            var scrollViewer = ListHelpers.GetScrollViewer(MainList);

            scrollViewer.ViewChanged += async (s, e) =>
            {
                if (!isLoadingMore && scrollViewer.VerticalOffset <= scrollViewer.ScrollableHeight - 50)
                {
                    isLoadingMore = true;

                    FooterProgress.Visibility = Visibility.Visible;

                    await foreach (var post in PostHelpers.GetFrontpageAsync(after: posts.Last().Post.Fullname, limit: 30))
                    {
                        posts.Add(post);
                    }

                    FooterProgress.Visibility = Visibility.Collapsed;

                    isLoadingMore = false;
                }
            };

            try
            {
                SecondPageFrame.Navigate(typeof(SidebarPage), "home", new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
            }
            catch (COMException)
            {

            }

            SubredditsList.ItemsSource = await Task.Run(() => App.RedditClient.Account.MySubscribedSubreddits(limit: 100));

            UserImage.Source = new BitmapImage(new Uri(await Task.Run(() => AccountController.GetImageUrl(App.RedditClient.Account.Me.UserData)), UriKind.Absolute));
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
