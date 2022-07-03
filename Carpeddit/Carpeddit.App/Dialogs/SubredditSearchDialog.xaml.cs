using Carpeddit.App.Collections;
using Carpeddit.App.Helpers;
using Carpeddit.App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Carpeddit.App.Dialogs
{
    public sealed partial class SubredditSearchDialog : ContentDialog
    {
        BulkConcurrentObservableCollection<PostViewModel> posts;
        string subredditName;
        string query;
        
        public SubredditSearchDialog(string subredditName, string query)
        {
            InitializeComponent();

            this.subredditName = subredditName;
            this.query = query;
            posts = new();
            Loaded += SubredditSearchDialog_Loaded;
        }

        private async void SubredditSearchDialog_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            PostsLoadingProgress.Visibility = Visibility.Visible;
            
            posts.AddRange(await Task.Run(() => GetPosts()));
            PostsList.ItemsSource = posts;

            PostsLoadingProgress.Visibility = Visibility.Collapsed;
        }

        public IEnumerable<PostViewModel> GetPosts(string before = "", string after = "")
        {
            var list = new List<PostViewModel>();
            var posts = App.RedditClient.Subreddit(subredditName).About().Search(q: query, limit: 50, before: before, after: after);

            foreach (var post in posts)
            {
                list.Add(new()
                {
                    Post = post,
                    Title = post.Title,
                    Description = post.GetDescription(),
                    Created = post.Created,
                    Subreddit = post.Subreddit,
                    Author = post.Author,
                    CommentsCount = post.Listing.NumComments
                });
            }

            return list;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.Visibility = Visibility.Collapsed;
                FooterProgress.Visibility = Visibility.Visible;

                posts.AddRange(await Task.Run(() => GetPosts(after: posts[posts.Count - 1].Post.Fullname)));

                button.Visibility = Visibility.Visible;
                FooterProgress.Visibility = Visibility.Collapsed;
            }
        }
    }
}