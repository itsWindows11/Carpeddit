using Carpeddit.App.Dialogs;
using Carpeddit.App.Pages.ModTools;
using Carpeddit.Common.Enums;
using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using muxc = Microsoft.UI.Xaml.Controls;

namespace Carpeddit.App.Pages
{
    public sealed partial class ModToolsPage : Page
    {
        public static Subreddit Subreddit { get; private set; }

        private readonly List<(string Tag, Type Page)> _pages = new()
        {
            ("queue", typeof(ModqueuePage)),
            ("reports", typeof(ModqueuePage)),
            ("spam", typeof(ModqueuePage)),
            ("edited", typeof(ModqueuePage)),
            ("unmoderated", typeof(ModqueuePage)),
            ("modlog", typeof(ModLogPage)),
            ("banned", typeof(BannedUsersPage)),
            ("muted", typeof(MutedUsersPage)),
            ("approved", typeof(ApprovedUsersPage)),
            ("moderators", typeof(ModeratorsListPage)),
            ("grantflair", null),
            ("userflair", typeof(UserFlairPage)),
            ("postflair", typeof(PostFlairPage)),
        };

        public ModToolsPage()
        {
            InitializeComponent();

            switch (App.SViewModel.ColorMode)
            {
                case 0:
                    ColorBrushBg.Color = Colors.Transparent;
                    break;
                case 1:
                    ColorBrushBg.Color = (Color)Resources["SystemAccentColor"];
                    break;
                case 2:
                    ColorBrushBg.Color = App.SViewModel.TintColorsList[App.SViewModel.TintColor];
                    break;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Subreddit subreddit)
                Subreddit = subreddit;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // Add handler for ContentFrame navigation.
            ContentFrame.Navigated += On_Navigated;

            // NavView doesn't load any page by default, so load home page.
            NavView.SelectedItem = NavView.MenuItems[0];

            // If navigation occurs on SelectionChanged, this isn't needed.
            // Because we use ItemInvoked to navigate, we need to call Navigate
            // here to load the home page.
            NavView_Navigate("queue", new Windows.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
        }

        private void NavView_ItemInvoked(muxc.NavigationView sender,
                                         muxc.NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_Navigate(
            string navItemTag,
            Windows.UI.Xaml.Media.Animation.NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;

            var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
            _page = item.Page;

            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = ContentFrame.CurrentSourcePageType;

            ModQueueType type = ModQueueType.Default;

            switch (navItemTag)
            {
                case "reports":
                    type = ModQueueType.Reports;
                    break;
                case "spam":
                    type = ModQueueType.Spam;
                    break;
                case "edited":
                    type = ModQueueType.Edited;
                    break;
                case "unmoderated":
                    type = ModQueueType.Unmoderated;
                    break;
                case "grantflair":
                    _ = new GrantUserFlairDialog().ShowAsync();
                    return;
            }

            // Only navigate if the selected page isn't currently loaded.
            if (_page is not null)
            {
                ContentFrame.Navigate(_page, _page == typeof(ModqueuePage) ? type : null, transitionInfo);
            }
        }

        private void NavView_BackRequested(muxc.NavigationView sender,
                                           muxc.NavigationViewBackRequestedEventArgs args)
        {
            TryGoBack();
        }

        private bool TryGoBack()
        {
            if (!ContentFrame.CanGoBack)
                return false;

            // Don't go back if the nav pane is overlayed.
            if (NavView.IsPaneOpen &&
                (NavView.DisplayMode == muxc.NavigationViewDisplayMode.Compact ||
                 NavView.DisplayMode == muxc.NavigationViewDisplayMode.Minimal))
                return false;

            ContentFrame.GoBack();
            return true;
        }

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            if (ContentFrame.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

                NavView.Header = ((muxc.NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();
            }
        }
    }
}
