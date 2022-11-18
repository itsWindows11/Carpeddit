using Carpeddit.App.Dialogs;
using Carpeddit.App.Helpers;
using Reddit.Controllers;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Carpeddit.App.Pages
{
    public sealed partial class SidebarPage : Page
    {
        public SidebarPage()
        {
            InitializeComponent();

            Loaded += SidebarPage_Loaded;            
        }

        private async void SidebarPage_Loaded(object sender, RoutedEventArgs e)
        {
            RecommendedSubredditsList.ItemsSource = await Task.Run(() => App.RedditClient.Account.MySubscribedSubreddits(limit: 5));
            MultisList.ItemsSource = await Task.Run(() => App.RedditClient.Account.Multis());

            LoggingHelper.LogInfo("[SidebarPage] Recommended subreddits loaded successfully.");
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is string str)
            {
                SubredditImageBorder.Visibility = Visibility.Collapsed;
                SubredditRulesExpander.Visibility = Visibility.Collapsed;
                
                switch (str)
                {
                    case "home":
                        AboutPageText.Text = "Home";
                        PageDescriptionText.Text = "Your personalized feed of posts based on the subreddits you follow.";
                        break;
                    case "popular":
                        AboutPageText.Text = "Popular";
                        PageDescriptionText.Text = "Popular posts pulled from the most active communities on Reddit.";
                        break;
                }

                AboutPageText.Margin = new Thickness(-8, 0, 0, 0);
            } else if (e.Parameter is Subreddit subreddit)
            {
                RecommendedSubredditsExpander.Visibility = Visibility.Collapsed;
                
                AboutPageText.Text = subreddit.SubredditData.DisplayNamePrefixed;
                PageDescriptionText.Text = subreddit.SubredditData.Description;
                
                AboutPageText.Margin = new Thickness(0);

                try
                {
                    SubredditImage.Source = new BitmapImage(new(subreddit.SubredditData.CommunityIcon.Replace("&amp;", "&")))
                    {
                        DecodePixelType = DecodePixelType.Logical
                    };
                }
                catch (UriFormatException)
                {

                }

                RulesList.ItemsSource = await Task.Run(() => subreddit.GetRules().Rules);

                LoggingHelper.LogInfo("[SidebarPage] Loaded subreddit info.");
            }
        }

        private void OnSubredditItemClick(object sender, RoutedEventArgs e)
            => (Window.Current.Content as Frame).Navigate(typeof(SubredditPage), App.RedditClient.Subreddit(name: ((sender as HyperlinkButton).Content as string).Replace("r/", "")).About());

        private void CreateMultiButton_Click(object sender, RoutedEventArgs e)
            => _ = new CreateMultiDialog().ShowAsync();
    }
}
