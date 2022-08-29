using Carpeddit.App.Helpers;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Carpeddit.App.Pages.Setup
{
    public sealed partial class SubscriptionPage : Page
    {
        public SubscriptionPage()
        {
            InitializeComponent();
        }

        private async void OnPrimaryActionClick(object sender, RoutedEventArgs e)
        {
            SubscribingProgressRing.Visibility = Visibility.Visible;

            try
            {
                var subreddit = App.RedditClient.Subreddit("Carpeddit");
                await subreddit.SubscribeAsync();
            } catch (Exception ex)
            {
                LoggingHelper.LogError("An error occurred.", ex);
            }

            SubscribingProgressRing.Visibility = Visibility.Collapsed;

            App.SViewModel.SetupProgress++;
            Frame.Navigate(typeof(FinishPage), new SlideNavigationTransitionInfo());
        }

        private void OnSecondaryActionClick(object sender, RoutedEventArgs e)
        {
            App.SViewModel.SetupProgress++;
            Frame.Navigate(typeof(FinishPage), new SlideNavigationTransitionInfo());
        }
    }
}
