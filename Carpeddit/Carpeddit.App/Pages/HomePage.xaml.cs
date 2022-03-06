using Carpeddit.App.Collections;
using Carpeddit.App.Models;
using Microsoft.Toolkit.Uwp;
using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Carpeddit.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        BulkConcurrentObservableCollection<PostViewModel> posts = new();
        int postsCount = 0;
        bool ArePostsLoaded = false;

        public HomePage()
        {
            InitializeComponent();
        }

        private async Task GetPostsAsync(string after = "", int limit = 13, string before = "")
        {
            List<Post> frontpage = App.RedditClient.GetFrontPage(limit: 13, after: after, before: before);

            foreach (Post post in frontpage)
            {
                posts.Add(new()
                {
                    Post = post
                });
            }

            for (postsCount = 0; postsCount < frontpage.Count; postsCount++);
            ArePostsLoaded = true;
        }

        private void UpvoteButton_Click(object sender, RoutedEventArgs e)
        {
            Post post = (sender as ToggleButton).Tag as Post;
            post.UpvoteAsync();
        }

        private void DownvoteButton_Click(object sender, RoutedEventArgs e)
        {
            Post post = (sender as ToggleButton).Tag as Post;
            post.DownvoteAsync();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.Visibility = Visibility.Collapsed;
                SampleProgress.Visibility = Visibility.Visible;
                await Task.Delay(500);
                await GetPostsAsync(after: posts[postsCount - 1].Post.Fullname).ConfigureAwait(false);
                SampleProgress.Visibility = Visibility.Collapsed;
                button.Visibility = Visibility.Visible;
            }
        }

        private async void MainList_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(500);
            await GetPostsAsync().ConfigureAwait(false);
            MainList.ItemsSource = posts;
        }
    }
}
