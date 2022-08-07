using Carpeddit.App.Controllers;
using Carpeddit.App.Pages;
using Reddit.Things;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using muxc = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Carpeddit.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// The current main page instance.
        /// NOTE: should be used carefully or else this might
        /// throw <see cref="NullReferenceException"/> when trying to access this.
        /// </summary>
        public static MainPage Current;

        private double NavViewCompactModeThresholdWidth { get => NavView.CompactModeThresholdWidth; }

        private bool _shouldDisplayModTools;

        public MainPage()
        {
            InitializeComponent();

            Current = this;

            var appViewTitleBar = ApplicationView.GetForCurrentView().TitleBar;

            appViewTitleBar.ButtonBackgroundColor = Colors.Transparent;
            appViewTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            Window.Current.SetTitleBar(AppTitleBar);

            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            Loaded += OnMainPageLoaded;

            App.SViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(App.SViewModel.TintColor) || e.PropertyName == nameof(App.SViewModel.ColorMode))
                {
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
            };

            NavigationCacheMode = NavigationCacheMode.Required;
        }

        private async void OnMainPageLoaded(object sender, RoutedEventArgs e)
        {
            if (App.RedditClient != null)
            {
                YourProfileItem.Content = await Task.Run(() => App.RedditClient.Account.Me.UserData.Name);
                ModerationToolsItem.Visibility = (await Task.Run(() => App.RedditClient.Account.Me.GetModeratedSubreddits(1) ?? new List<ModeratedListItem>())).Any() ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            AppTitleBar.Visibility = sender.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            //throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        // List of ValueTuple holding the Navigation Tag and the relative Navigation Page
        private readonly List<(string Tag, Type Page)> _pages = new()
        {
            ("home", typeof(HomePage)),
            ("popular", typeof(PopularPostsPage)),
            ("collections", typeof(CollectionsPage)),
            ("mailbox", typeof(MailboxPage)),
            ("your_profile", typeof(YourProfilePage)),
        };

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // Add handler for ContentFrame navigation.
            ContentFrame.Navigated += On_Navigated;

            // NavView doesn't load any page by default, so load home page.
            NavView.SelectedItem = NavView.MenuItems[0];
            // If navigation occurs on SelectionChanged, this isn't needed.
            // Because we use ItemInvoked to navigate, we need to call Navigate
            // here to load the home page.
            NavView_Navigate("home", new Windows.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());

            // Listen to the window directly so the app responds
            // to accelerator keys regardless of which element has focus.
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
                CoreDispatcher_AcceleratorKeyActivated;

            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;

            SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;

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

        private void NavView_ItemInvoked(muxc.NavigationView sender,
                                         muxc.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                NavView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
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
            if (navItemTag == "settings")
            {
                _page = typeof(SettingsPage);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }
            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = ContentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (_page is not null && !Equals(preNavPageType, _page))
            {
                ContentFrame.Navigate(_page, null, transitionInfo);
            }
        }

        private void NavView_BackRequested(muxc.NavigationView sender,
                                           muxc.NavigationViewBackRequestedEventArgs args)
        {
            TryGoBack();
        }

        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
        {
            // When Alt+Left are pressed navigate back
            if (e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown
                && e.VirtualKey == VirtualKey.Left
                && e.KeyStatus.IsMenuKeyDown == true
                && !e.Handled)
            {
                e.Handled = TryGoBack();
            }
        }

        private void System_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = TryGoBack();
            }
        }

        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs e)
        {
            // Handle mouse back button.
            if (e.CurrentPoint.Properties.IsXButton1Pressed)
            {
                e.Handled = TryGoBack();
            }
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
            if (ContentFrame.SourcePageType == typeof(SettingsPage))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                NavView.SelectedItem = (muxc.NavigationViewItem)NavView.SettingsItem;
                NavViewHeader.Text = "Settings";
            }
            else if (ContentFrame.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

                if (ContentFrame.SourcePageType == typeof(YourProfilePage))
                {
                    if (e.Parameter == null)
                    {
                        try
                        {
                            NavView.SelectedItem = NavView.MenuItems
                            .OfType<muxc.NavigationViewItem>()
                            .First(n => n.Tag.Equals(item.Tag));
                        }
                        catch (InvalidOperationException)
                        {
                            try
                            {
                                NavView.SelectedItem = NavView.FooterMenuItems
                                .OfType<muxc.NavigationViewItem>()
                                .First(n => n.Tag.Equals(item.Tag));
                            }
                            catch (InvalidOperationException)
                            {
                                Debug.WriteLine("Cannot navigate...");
                            }
                        }
                    }
                } else
                {
                    try
                    {
                        NavView.SelectedItem = NavView.MenuItems
                        .OfType<muxc.NavigationViewItem>()
                        .First(n => n.Tag.Equals(item.Tag));
                    }
                    catch (InvalidOperationException)
                    {
                        try
                        {
                            NavView.SelectedItem = NavView.FooterMenuItems
                            .OfType<muxc.NavigationViewItem>()
                            .First(n => n.Tag.Equals(item.Tag));
                        }
                        catch (InvalidOperationException)
                        {
                            Debug.WriteLine("Cannot navigate...");
                        }
                    }
                }

                if (ContentFrame.SourcePageType == typeof(YourProfilePage)) {
                    NavViewHeader.Text = e.Parameter == null ? "Your profile" : "Profile";
                } else if (ContentFrame.SourcePageType == typeof(SearchResultsPage))
                {
                    NavViewHeader.Text = "Search results";
                } else
                {
                    NavViewHeader.Text = ((muxc.NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();
                }
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width >= 641)
            {
                AppTitleBar.Margin = new Thickness(52, 7, 290, 0);
                AccountMenuBtn.Visibility = App.SViewModel.ShowAccountBtnInTitleBar ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (e.NewSize.Width < 641)
            {
                AppTitleBar.Margin = new Thickness(95, 7, 290, 0);
                AccountMenuBtn.Visibility = Visibility.Collapsed;
            }
        }

        private async void LogoutFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new()
            {
                Title = "Log out",
                Content = "Are you sure that you want to log out?",
                SecondaryButtonStyle = Resources["AccentButtonStyle"] as Style,
                SecondaryButtonText = "Cancel",
                PrimaryButtonText = "Log out"
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                NavigationCacheMode = NavigationCacheMode.Disabled;
                await AccountController.LogOutAsync();
                (Window.Current.Content as Frame).Navigate(typeof(LoginPage));
            }
        }

        private void NavViewSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(sender.Text))
            {
                ContentFrame.Navigate(typeof(SearchResultsPage), sender.Text);
            }
        }

        private void ProfileFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(YourProfilePage));
        }

        private void OnModerationToolsItemClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ModToolsPage), App.RedditClient.Subreddit("mod"));
        }
    }
}
