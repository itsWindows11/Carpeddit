using Carpeddit.App.Collections;
using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
            } else if (e.Parameter is Subreddit subreddit)
            {
                RecommendedSubredditsExpander.Visibility = Visibility.Collapsed;
                
                AboutPageText.Text = subreddit.SubredditData.DisplayNamePrefixed;
                PageDescriptionText.Text = subreddit.SubredditData.Description;

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
            }
        }

        private void OnSubredditItemClick(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(SubredditPage), App.RedditClient.Subreddit(name: ((sender as HyperlinkButton).Content as string).Replace("r/", "")).About());
        }
    }
}
