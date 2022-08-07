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

            await foreach (var post in GetModqueueAsync())
            {
                _posts.Add(post);
            }

            MainList.ItemsSource = _posts;
            MainList.Visibility = Visibility.Visible;
            Progress.Visibility = Visibility.Collapsed;

            if (_posts.Count == 0)
            {
                NoModQueueItems.Visibility = Visibility.Visible;
                MainList.Visibility = Visibility.Collapsed;
            }
        }

        public async IAsyncEnumerable<PostViewModel> GetModqueueAsync(string before = "", string after = "", int limit = 100, ModQueueType type = ModQueueType.Default)
        {
            List<Post> list = type switch
            {
                ModQueueType.Edited => await Task.Run(() => subreddit.Posts.GetModQueueEdited(limit: limit, after: after, before: before)),
                ModQueueType.Reports => await Task.Run(() => subreddit.Posts.GetModQueueReports(limit: limit, after: after, before: before)),
                ModQueueType.Spam => await Task.Run(() => subreddit.Posts.GetModQueueSpam(limit: limit, after: after, before: before)),
                ModQueueType.Unmoderated => await Task.Run(() => subreddit.Posts.GetModQueueUnmoderated(limit: limit, after: after, before: before)),
                _ => await Task.Run(() => subreddit.Posts.GetModQueue(limit: limit, after: after, before: before)),
            };

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

        private async void LoadMoreButton_Click(object sender, RoutedEventArgs e)
        {
            await foreach (var post in GetModqueueAsync(after: _posts.Last().Post.Fullname, type: type))
            {
                _posts.Add(post);
            }
        }
    }
}