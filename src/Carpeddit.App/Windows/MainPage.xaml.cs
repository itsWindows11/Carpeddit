using Carpeddit.Api.Helpers;
using Carpeddit.Api.Services;
using Carpeddit.Api.Watchers;
using Carpeddit.App.ViewModels;
using Carpeddit.App.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Windows.ApplicationModel.Background;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using static Carpeddit.Api.Watchers.MailboxWatcher;
using muxc = Microsoft.UI.Xaml.Controls;

namespace Carpeddit.App
{
    public sealed partial class MainPage : Page
    {
        private SettingsViewModel SViewModel { get; } = App.Services.GetService<SettingsViewModel>();

        private readonly List<(string Tag, Type Page)> _pages = new()
        {
            ("home", typeof(HomePage)),
            ("popular", typeof(PopularPage)),
            ("collections", typeof(CollectionsPage)),
            ("mailbox", typeof(MailboxPage)),
            ("profile", typeof(ProfilePage)),
        };

        private IRedditService service = App.Services.GetService<IRedditService>();

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            Loaded += OnLoaded;
            TitleBar.Loaded += (_, _1) => TitleBar.SetAsTitleBar();
        }
    }

    public partial class MainPage
    {
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            _ = service.GetMeAsync().ContinueWith(async (t) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var user = t.Result;

                    ProfileBitmap.UriSource = new(WebUtility.HtmlDecode(user.IconImage));
                    
                    UnreadBadge.Visibility = ((user.HasMail ?? false) || (user.HasModMail ?? false)) ? Visibility.Visible : Visibility.Collapsed;
                    
                    if (user.InboxCount > 0)
                        UnreadBadge.Value = user.InboxCount;
                });
            });

            ContentFrame.Navigated += OnNavigated;

            NavView.SelectedItem = NavView.MenuItems[0];

            Navigate("home", new EntranceNavigationTransitionInfo());

            // Listen to the window directly so the app responds
            // to accelerator keys regardless of which element has focus.
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += OnAcceleratorKeyActivated;
            Window.Current.CoreWindow.PointerPressed += OnCoreWindowPointerPressed;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnSystemBackRequested;

            foreach (var registration in BackgroundTaskRegistration.AllTasks)
            {
                registration.Value.Unregister(true);
            }

            var requestStatus = await BackgroundExecutionManager.RequestAccessAsync();

            if (requestStatus != BackgroundAccessStatus.AllowedSubjectToSystemPolicy && requestStatus != BackgroundAccessStatus.AlwaysAllowed)
            {
                System.Diagnostics.Debug.WriteLine("Cannot use background tasks.");
            }
            else
            {
                var builder = new BackgroundTaskBuilder
                {
                    Name = "Carpeddit - Notifications Background Task"
                };

                var trigger = new ApplicationTrigger();

                builder.SetTrigger(trigger);
                _ = builder.Register();

                await trigger.RequestAsync();
            }
        }

        private void OnLogOutClick(object sender, RoutedEventArgs e)
        {
            NavigationCacheMode = NavigationCacheMode.Disabled;
            _ = AccountHelper.Instance.SignOutAsync();
            Frame.Navigate(typeof(LoginPage));
        }

        private void OnItemInvoked(muxc.NavigationView _, muxc.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
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
                ContentFrame.Navigate(_page, null, transitionInfo);
        }

        private void OnBackRequested(muxc.NavigationView _, muxc.NavigationViewBackRequestedEventArgs _1)
            => TryGoBack();

        private void OnAcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
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

        private void OnSystemBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled)
                e.Handled = TryGoBack();
        }

        private void OnCoreWindowPointerPressed(CoreWindow sender, PointerEventArgs e)
        {
            // Handle mouse back button.
            if (e.CurrentPoint.Properties.IsXButton1Pressed)
                e.Handled = TryGoBack();
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

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (ContentFrame.SourcePageType == typeof(SettingsPage))
            {
                NavView.SelectedItem = (muxc.NavigationViewItem)NavView.SettingsItem;
            }
            else if (ContentFrame.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

                if (ContentFrame.SourcePageType == typeof(ProfilePage))
                {
                    if (e.Parameter != null)
                        return;

                    var navItem = NavView.MenuItems.OfType<muxc.NavigationViewItem>()
                        .FirstOrDefault(i => i.Tag.Equals(item.Tag));

                    if (navItem != null)
                    {
                        NavView.SelectedItem = navItem;
                        return;
                    }

                    navItem = NavView.FooterMenuItems.OfType<muxc.NavigationViewItem>()
                        .FirstOrDefault(i => i.Tag.Equals(item.Tag));

                    if (navItem != null)
                        NavView.SelectedItem = navItem;
                }
            }
        }
    }
}