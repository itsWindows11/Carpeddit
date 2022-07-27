using Carpeddit.App.Collections;
using Carpeddit.App.Dialogs;
using Carpeddit.App.Helpers;
using Carpeddit.App.Models;
using Reddit.Controllers;
using Reddit.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

namespace Carpeddit.App.Pages
{
    public sealed partial class YourProfilePage : Page
    {
        private BulkConcurrentObservableCollection<PostViewModel> posts;
        private BulkConcurrentObservableCollection<CommentViewModel> comments;
        private BulkConcurrentObservableCollection<ModeratedListItem> myModSubreddits;
        private Reddit.Controllers.User user = App.RedditClient.Account.Me;

        public YourProfilePage()
        {
            InitializeComponent();

            posts = new();
            comments = new();
            myModSubreddits = new();

            Loaded += Page_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Account account)
            {
                user = account.Me;
            } else if (e.Parameter is Reddit.Controllers.User _user)
            {
                user = _user;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.Visibility = Visibility.Collapsed;
                FooterProgress.Visibility = Visibility.Visible;

                LoggingHelper.LogInfo($"[YourProfilePage] Loading more posts...");

                posts.AddRange(await Task.Run(() => GetPosts(after: posts[posts.Count - 1].Post.Fullname) ?? new List<PostViewModel>()));

                LoggingHelper.LogInfo($"[YourProfilePage] Loaded more posts.");

                button.Visibility = Visibility.Visible;
                FooterProgress.Visibility = Visibility.Collapsed;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Page_Loaded;

            if (user != App.RedditClient.Account.Me)
            {
                FloatingSplit.Visibility = Visibility.Collapsed;
            } else
            {
                FloatingSplit.Visibility = Visibility.Visible;
            }

            LoadMoreButton.Visibility = Visibility.Collapsed;
            ProgressR.Visibility = Visibility.Visible;

            LoggingHelper.LogInfo($"[YourProfilePage] Loading posts...");

            posts.AddRange(await Task.Run(() => GetPosts() ?? new List<PostViewModel>()));

            LoggingHelper.LogInfo($"[YourProfilePage] Loaded posts.");
            LoggingHelper.LogInfo($"[YourProfilePage] Loading comments...");

            comments.AddRange(await Task.Run(() => GetComments() ?? new List<CommentViewModel>()));

            LoggingHelper.LogInfo($"[YourProfilePage] Loaded comments.");
            LoggingHelper.LogInfo($"[YourProfilePage] Loading your moderated subreddits...");

            myModSubreddits.AddRange(await Task.Run(() => user.GetModeratedSubreddits(100) ?? new List<ModeratedListItem>()));

            LoggingHelper.LogInfo($"[YourProfilePage] Loaded your moderated subreddits...");

            MainList.ItemsSource = posts;
            CommentsList.ItemsSource = comments;
            MyModeratingSubredditsList.ItemsSource = myModSubreddits;

            ProgressR.Visibility = Visibility.Collapsed;
            LoadMoreButton.Visibility = Visibility.Visible;
        }

        private IEnumerable<PostViewModel> GetPosts(string after = "", int limit = 100, string before = "")
        {
            List<Reddit.Controllers.Post> frontpage = user.GetPostHistory(limit: limit, after: after, before: before);
            List<PostViewModel> posts1 = new();

            foreach (Reddit.Controllers.Post post in frontpage)
            {
                posts1.Add(new()
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

            return posts1;
        }

        private IEnumerable<CommentViewModel> GetComments(string after = "", int limit = 100, string before = "")
        {
            List<Reddit.Controllers.Comment> frontpage = user.GetCommentHistory(limit: limit, after: after, before: before);

            List<CommentViewModel> comments1 = new();

            foreach (Reddit.Controllers.Comment comment in frontpage)
            {
                comments1.Add(new()
                {
                    OriginalComment = comment
                });
            }

            return comments1;
        }

        private async void CreatePostItem_Click(object sender, RoutedEventArgs e)
        {
            _ = await new CreatePostDialog().ShowAsync();
        }

        private void NavigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            switch (args.InvokedItemContainer.Tag)
            {
                case "posts":
                    MyModeratingSubredditsList.Visibility = Visibility.Collapsed;
                    CommentsList.Visibility = Visibility.Collapsed;
                    MainList.Visibility = Visibility.Visible;
                    break;
                case "comments":
                    MyModeratingSubredditsList.Visibility = Visibility.Collapsed;
                    CommentsList.Visibility = Visibility.Visible;
                    MainList.Visibility = Visibility.Collapsed;
                    break;
                case "modSubreddits":
                    MyModeratingSubredditsList.Visibility = Visibility.Visible;
                    CommentsList.Visibility = Visibility.Collapsed;
                    MainList.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private async void LoadMoreButton1_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.Visibility = Visibility.Collapsed;
                FooterProgress1.Visibility = Visibility.Visible;

                LoggingHelper.LogInfo($"[YourProfilePage] Loading more comments...");

                comments.AddRange(await Task.Run(() => GetComments(after: comments[comments.Count - 1].OriginalComment.Fullname)));

                LoggingHelper.LogInfo($"[YourProfilePage] Loaded more comments.");

                button.Visibility = Visibility.Visible;
                FooterProgress1.Visibility = Visibility.Collapsed;
            }
        }

        private async void UpvoteButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            CommentViewModel comment = (e.OriginalSource as FrameworkElement).DataContext as CommentViewModel;

            if (toggle.IsChecked ?? false)
            {
                await comment.OriginalComment.UpvoteAsync();
                comment.Upvoted = true;
                comment.Downvoted = false;
                comment.RawVoteRatio += 1;
            }
            else
            {
                await comment.OriginalComment.UnvoteAsync();
                comment.Upvoted = false;
                comment.Downvoted = false;
            }
        }

        private async void DownvoteButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            CommentViewModel comment = (e.OriginalSource as FrameworkElement).DataContext as CommentViewModel;

            if (toggle.IsChecked ?? false)
            {
                await comment.OriginalComment.DownvoteAsync();
                comment.Upvoted = false;
                comment.Downvoted = true;
                comment.RawVoteRatio -= 1;
            }
            else
            {
                await comment.OriginalComment.UnvoteAsync();
                comment.Upvoted = false;
                comment.Downvoted = false;
            }
        }

        private async void DeleteCommentButton_Click(object sender, RoutedEventArgs e)
        {
            CommentViewModel comment = (e.OriginalSource as FrameworkElement).DataContext as CommentViewModel;

            // Delete the comment and remove it from the list.
            await comment.OriginalComment.DeleteAsync();

            comments.Remove(comment);
        }
    }
}
