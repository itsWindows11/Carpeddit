using Carpeddit.App.Collections;
using Carpeddit.App.Helpers;
using Carpeddit.App.Models;
using Reddit.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Pages
{
    public sealed partial class SavedPostsPage : Page
    {
        private BulkConcurrentObservableCollection<PostViewModel> savedPosts;

        public SavedPostsPage()
        {
            InitializeComponent();

            savedPosts = new();
            Loaded += SavedPostsPage_Loaded;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.Visibility = Visibility.Collapsed;
                FooterProgress.Visibility = Visibility.Visible;

                var posts1 = await Task.Run(() => GetPosts(after: savedPosts[savedPosts.Count - 1].Post.Fullname));

                savedPosts.AddRange(posts1);

                button.Visibility = Visibility.Visible;
                FooterProgress.Visibility = Visibility.Collapsed;
            }
        }

        private async void SavedPostsPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadingProgress.Visibility = Visibility.Visible;

            savedPosts.AddRange(await Task.Run(() => GetPosts()));
            SavedPostsList.ItemsSource = savedPosts;

            LoadingProgress.Visibility = Visibility.Collapsed;
        }

        private IEnumerable<PostViewModel> GetPosts(string after = "", int limit = 100, string before = "")
        {
            List<Post> frontpage = App.RedditClient.Account.Me.GetPostHistory(where: "saved", limit: limit, after: after, before: before);
            List<PostViewModel> postViews = new();

            foreach (Post post in frontpage)
            {
                PostViewModel vm = new()
                {
                    Post = post,
                    Title = post.Title,
                    Description = post.GetDescription(),
                    Created = post.Created,
                    Subreddit = post.Subreddit,
                    Author = post.Author,
                    CommentsCount = post.Listing.NumComments
                };

                postViews.Add(vm);
            }

            return postViews;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }
    }
}
