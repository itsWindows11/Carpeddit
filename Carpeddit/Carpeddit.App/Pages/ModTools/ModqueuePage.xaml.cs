using Carpeddit.App.Collections;
using Carpeddit.App.Helpers;
using Carpeddit.App.Models;
using Carpeddit.Common.Enums;
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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Carpeddit.App.Pages.ModTools
{
    public sealed partial class ModqueuePage : Page
    {
        private BulkConcurrentObservableCollection<PostViewModel> _posts;

        private Subreddit subreddit => ModToolsPage.Subreddit;

        ModQueueType type;

        public ModqueuePage()
        {
            InitializeComponent();

            _posts = new();
            Loaded += ModqueuePage_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            type = (ModQueueType)e.Parameter;
        }

        private async void ModqueuePage_Loaded(object sender, RoutedEventArgs e)
        {
            Progress.Visibility = Visibility.Visible;
            _posts.AddRange(await Task.Run(() => GetPosts(type: type)));
            MainList.ItemsSource = _posts;
            MainList.Visibility = Visibility.Visible;
            Progress.Visibility = Visibility.Collapsed;

            if (_posts.Count == 0)
            {
                NoModQueueItems.Visibility = Visibility.Visible;
                MainList.Visibility = Visibility.Collapsed;
            }
        }

        public List<PostViewModel> GetPosts(string before = "", string after = "", int limit = 100, ModQueueType type = ModQueueType.Default)
        {
            List<Post> list = type switch
            {
                ModQueueType.Edited => subreddit.Posts.GetModQueueEdited(limit: limit, after: after, before: before),
                ModQueueType.Reports => subreddit.Posts.GetModQueueReports(limit: limit, after: after, before: before),
                ModQueueType.Spam => subreddit.Posts.GetModQueueSpam(limit: limit, after: after, before: before),
                ModQueueType.Unmoderated => subreddit.Posts.GetModQueueUnmoderated(limit: limit, after: after, before: before),
                _ => subreddit.Posts.GetModQueue(limit: limit, after: after, before: before),
            };

            List<PostViewModel> postViews = new();

            foreach (Post post in list)
            {
                postViews.Add(new()
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

            return postViews;
        }

        private async void LoadMoreButton_Click(object sender, RoutedEventArgs e)
        {
            _posts.AddRange(await Task.Run(() => GetPosts(after: _posts[_posts.Count - 1].Post.Fullname, type: type)));
        }
    }
}