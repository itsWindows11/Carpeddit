using Carpeddit.App.Collections;
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

        public ModqueuePage()
        {
            InitializeComponent();

            _posts = new();
            Loaded += ModqueuePage_Loaded;
        }

        private async void ModqueuePage_Loaded(object sender, RoutedEventArgs e)
        {
            _posts.AddRange(await Task.Run(() => GetPosts()));
            MainList.ItemsSource = _posts;
        }

        public List<PostViewModel> GetPosts(string before = "", string after = "", int limit = 24, ModQueueType type = ModQueueType.Default)
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
                    Description = GetPostDesc(post),
                    Created = post.Created,
                    Subreddit = post.Subreddit,
                    Author = post.Author,
                    CommentsCount = post.Comments.GetComments().Count
                });
            }

            return postViews;
        }

        private string GetPostDesc(Post post)
        {
            if (post is LinkPost linkPost)
            {
                return linkPost.URL;
            }
            else if (post is SelfPost selfPost)
            {
                return selfPost.SelfText;
            }

            return "No content";
        }

        private async void LoadMoreButton_Click(object sender, RoutedEventArgs e)
        {
            _posts.AddRange(await Task.Run(() => GetPosts(after: _posts[_posts.Count - 1].Post.Fullname)));
        }
    }
}