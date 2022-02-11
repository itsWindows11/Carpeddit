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
        private ObservableCollection<PostViewModel> posts = new();
        private string chosenFilter;

        public PopularPostsPage()
        {
            InitializeComponent();

            _ = InitAsync();
        }

        public async Task InitAsync()
        {
            foreach (Post post in App.RedditClient.Subreddit("all").Posts.GetNew(limit: 13))
            {
                posts.Add(new PostViewModel()
                {
                    Post = post
                });
            }
            Debug.WriteLine(posts[0]);
            Debug.WriteLine(MainList.Items[0]);
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            posts.Clear();
            switch ((sender as ComboBox).SelectedIndex)
            {
                case 0:
                    // New
                    foreach (Post post in App.RedditClient.Subreddit("all").Posts.New)
                    {
                        posts.Add(new PostViewModel() { Post = post });
                    }
                    chosenFilter = "New";
                    MainList.ItemsSource = posts;
                    break;
                case 1:
                    // Hot
                    foreach (Post post in App.RedditClient.Subreddit("all").Posts.Hot)
                    {
                        posts.Add(new PostViewModel() { Post = post });
                    }
                    chosenFilter = "Hot";
                    MainList.ItemsSource = posts;
                    break;
                case 2:
                    // Top
                    foreach (Post post in App.RedditClient.Subreddit("all").Posts.Top)
                    {
                        posts.Add(new PostViewModel()
                        {
                            Post = post
                        });
                    }
                    chosenFilter = "Top";
                    MainList.ItemsSource = posts;
                    break;
                case 3:
                    // Rising
                    foreach (Post post in App.RedditClient.Subreddit("all").Posts.Rising)
                    {
                        posts.Add(new PostViewModel()
                        {
                            Post = post
                        });
                    }
                    chosenFilter = "Rising"; // fun fact: this string once used to hold a value of "Rising Media Player"
                    MainList.ItemsSource = posts;
                    break;
                case 4:
                    // Controversial
                    foreach (Post post in App.RedditClient.Subreddit("all").Posts.Controversial)
                    {
                        posts.Add(new PostViewModel() { Post = post });
                    }
                    chosenFilter = "Controversial";
                    MainList.ItemsSource = posts;
                    break;
            }
        }
    }
}
