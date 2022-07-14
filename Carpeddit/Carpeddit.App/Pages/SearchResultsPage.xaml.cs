using Carpeddit.App.Helpers;
using Carpeddit.App.Models;
using Reddit.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Carpeddit.App.Pages
{
    public sealed partial class SearchResultsPage : Page
    {
        private string query = string.Empty;

        public SearchResultsPage()
        {
            InitializeComponent();

            Loaded += SearchResultsPageLoaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is string str)
            {
                query = str;
            }
        }

        private async void SearchResultsPageLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= SearchResultsPageLoaded;
            LoadingProgress.Visibility = Visibility.Visible;

            List<Post> posts = await GetPostsAsync();
            List<User> users = await GetUsersAsync();
            List<Subreddit> subreddits = await GetSubredditsAsync();

            List<PostViewModel> posts2 = new();

            foreach (Post post in posts)
            {
                PostViewModel post1 = new()
                {
                    Post = post,
                    Title = post.Title,
                    Description = post.GetDescription(),
                    Created = post.Created,
                    Subreddit = post.Subreddit,
                    Author = post.Author,
                    CommentsCount = post.Listing.NumComments
                };

                posts2.Add(post1);
            }

            PostsList.ItemsSource = posts2;
            PeopleList.ItemsSource = users;
            SubredditsList.ItemsSource = subreddits;

            LoadingProgress.Visibility = Visibility.Collapsed;
        }

        private Task<List<Post>> GetPostsAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    return App.RedditClient.Search(new Reddit.Inputs.Search.SearchGetSearchInput(query, false, "relevance"));
                }
                catch
                {

                }
                return new();
            });
        }

        private Task<List<User>> GetUsersAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    return App.RedditClient.SearchUsers(new Reddit.Inputs.Search.SearchGetSearchInput(query));
                }
                catch
                {

                }
                return new();
            });
        }

        private Task<List<Subreddit>> GetSubredditsAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    return App.RedditClient.SearchSubreddits(new Reddit.Inputs.Search.SearchGetSearchInput(query));
                }
                catch
                {

                }
                return new();
            });
        }

        private void NavigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer is Microsoft.UI.Xaml.Controls.NavigationViewItem item)
            {
                switch (item.Tag as string)
                {
                    case "posts":
                        PostsList.Visibility = Visibility.Visible;
                        PeopleList.Visibility = Visibility.Collapsed;
                        SubredditsList.Visibility = Visibility.Collapsed;
                        break;
                    case "communities":
                        PostsList.Visibility = Visibility.Collapsed;
                        PeopleList.Visibility = Visibility.Collapsed;
                        SubredditsList.Visibility = Visibility.Visible;
                        break;
                    case "people":
                        PostsList.Visibility = Visibility.Collapsed;
                        PeopleList.Visibility = Visibility.Visible;
                        SubredditsList.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        private void UserHyperlink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            string text = (sender.Inlines[1] as Windows.UI.Xaml.Documents.Run).Text;
            if (!text.Contains("[deleted]"))
            {
                MainPage.Current.ContentFrame.Navigate(typeof(YourProfilePage), App.RedditClient.User(text).About());
            }
        }

        private void SubredditHyperlink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            string text = (sender.Inlines[1] as Windows.UI.Xaml.Documents.Run).Text;
            if (Window.Current.Content is Frame rootFrame)
            {
                rootFrame.Navigate(typeof(SubredditPage), App.RedditClient.Subreddit(text).About());
            }
        }
    }
}
