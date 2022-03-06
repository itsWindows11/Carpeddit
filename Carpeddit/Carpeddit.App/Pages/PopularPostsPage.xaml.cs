using Carpeddit.App.Collections;
using Carpeddit.App.Models;
using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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
    public sealed partial class PopularPostsPage : Page
    {
        BulkConcurrentObservableCollection<PostViewModel> posts = new();
        int postsCount = 0;
        bool ArePostsLoaded = false;
        string chosenFilter = "Hot";

        public PopularPostsPage()
        {
            InitializeComponent();

        }

        private async Task GetPostsAsync(string after = "", int limit = 13, string before = "")
        {
            List<Post> frontpage = App.RedditClient.Subreddit("all").Posts.GetHot(limit: 13, after: after, before: before);

            foreach (Post post in frontpage)
            {
                posts.Add(new()
                {
                    Post = post
                });
            }

            for (postsCount = 0; postsCount < frontpage.Count; postsCount++) ;
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            posts.Clear();
            switch ((sender as ComboBox).SelectedIndex)
            {
                case 0:
                    // New
                    chosenFilter = "New";
                    break;
                case 1:
                    // Hot
                    chosenFilter = "Hot";
                    break;
                case 2:
                    // Top
                    chosenFilter = "Top";
                    break;
                case 3:
                    // Rising
                    chosenFilter = "Rising"; // fun fact: this string once used to hold a value of "Rising Media Player"
                    break;
                case 4:
                    // Controversial
                    chosenFilter = "Controversial";
                    break;
            }
        }
    }
}
