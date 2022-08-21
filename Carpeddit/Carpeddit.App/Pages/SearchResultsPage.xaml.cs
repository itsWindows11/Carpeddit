using Carpeddit.App.Helpers;
using Carpeddit.App.ViewModels;
using Reddit.Controllers;
using Reddit.Inputs.Search;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

            List<PostViewModel> posts = new();
            List<User> users = new();
            List<Subreddit> subreddits = new();

            await foreach (var post in GetPostsAsync())
            {
                posts.Add(post);
            }

            await foreach (var user in GetUsersAsync())
            {
                users.Add(user);
            }

            await foreach (var post in GetSubredditsAsync())
            {
                subreddits.Add(post);
            }

            PostsList.ItemsSource = posts;
            PeopleList.ItemsSource = users;
            SubredditsList.ItemsSource = subreddits;

            LoadingProgress.Visibility = Visibility.Collapsed;
        }

        private async IAsyncEnumerable<PostViewModel> GetPostsAsync()
        {
            List<Post> list = await Task.Run(() => App.RedditClient.Search(new SearchGetSearchInput(query, false, "relevance")));

            foreach (Post post in list)
            {
                yield return new()
                {
                    Post = post,
                    Title = post.Title,
                    Description = post.GetDescription(),
                    Created = post.Created,
                    Subreddit = post.Subreddit,
                    Author = post.Author,
                    CommentsCount = post.Listing.NumComments
                };
            }
        }

        private async IAsyncEnumerable<User> GetUsersAsync()
        {
            List<User> list = await Task.Run(() => App.RedditClient.SearchUsers(new SearchGetSearchInput(query)));

            foreach (var user in list)
            {
                yield return user;
            }
        }

        private async IAsyncEnumerable<Subreddit> GetSubredditsAsync()
        {
            List<Subreddit> list = await Task.Run(() => App.RedditClient.SearchSubreddits(new SearchGetSearchInput(query)));

            foreach (var subreddit in list)
            {
                yield return subreddit;
            }
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
