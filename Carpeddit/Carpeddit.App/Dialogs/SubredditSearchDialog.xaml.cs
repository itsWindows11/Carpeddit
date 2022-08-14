using Carpeddit.App.Collections;
using Carpeddit.App.Helpers;
using Carpeddit.App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using System;
using Windows.ApplicationModel.DataTransfer;
using Carpeddit.App.Pages;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.System;

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
            SuggestBox.PlaceholderText = "Search r/" + subredditName;
            SuggestBox.Text = query; 
            Loaded += SubredditSearchDialog_Loaded;
        }

        private async void SubredditSearchDialog_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            PostsList.Visibility = Visibility.Collapsed;
            PostsLoadingProgress.Visibility = Visibility.Visible;
            
            posts.AddRange(await Task.Run(() => GetPosts()));
            PostsList.ItemsSource = posts;

            PostsLoadingProgress.Visibility = Visibility.Collapsed;
            PostsList.Visibility = Visibility.Visible;
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

        private async void OnSuggestBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            query = sender.Text;

            PostsList.Visibility = Visibility.Collapsed;
            PostsLoadingProgress.Visibility = Visibility.Visible;

            posts.Clear();
            posts.AddRange(await Task.Run(() => GetPosts()));
            PostsList.ItemsSource = posts;

            PostsLoadingProgress.Visibility = Visibility.Collapsed;
            PostsList.Visibility = Visibility.Visible;
        }
    }

    public partial class SubredditSearchDialog
    {
        private async void MarkdownLinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out Uri link))
            {
                _ = await Launcher.LaunchUriAsync(link);
                return;
            }

            if (e.Link.StartsWith("/r/"))
            {
                (Window.Current.Content as Frame).Navigate(typeof(SubredditPage), App.RedditClient.Subreddit(name: e.Link.Substring(3)).About());
                return;
            }

            if (e.Link.StartsWith("r/"))
            {
                (Window.Current.Content as Frame).Navigate(typeof(SubredditPage), App.RedditClient.Subreddit(name: e.Link.Substring(2)).About());
                return;
            }

            if (e.Link.StartsWith("/u/"))
            {
                MainPage.Current.ContentFrame.Navigate(typeof(YourProfilePage), App.RedditClient.User(name: e.Link.Substring(3)).About());
                return;
            }

            if (e.Link.StartsWith("u/"))
            {
                MainPage.Current.ContentFrame.Navigate(typeof(YourProfilePage), App.RedditClient.User(name: e.Link.Substring(2)).About());
                return;
            }
        }

        private async void UpvoteButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            PostViewModel post = toggle.Tag as PostViewModel;

            if (toggle.IsChecked ?? false)
            {
                await post.Post.UpvoteAsync();
                post.Upvoted = true;
                post.Downvoted = false;
            }
            else
            {
                await post.Post.UnvoteAsync();
                post.Upvoted = false;
                post.Downvoted = false;
            }
        }

        private async void DownvoteButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            PostViewModel post = toggle.Tag as PostViewModel;

            if (toggle.IsChecked ?? false)
            {
                await post.Post.DownvoteAsync();
                post.Upvoted = false;
                post.Downvoted = true;
            }
            else
            {
                await post.Post.UnvoteAsync();
                post.Upvoted = false;
                post.Downvoted = false;
            }
        }

        private void Title_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame && sender is TextBlock text && text.Tag is PostViewModel post)
            {
                rootFrame.Navigate(typeof(PostDetailsPage), post);
                Hide();
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

        private async void RemovePostButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement).DataContext is PostViewModel post)
            {
                await post.Post.RemoveAsync();

                (sender as HyperlinkButton).Content = "Removed";
                (sender as HyperlinkButton).IsEnabled = false;

                (((sender as HyperlinkButton).Parent as StackPanel).Children[5] as HyperlinkButton).Content = "Approve";
                (((sender as HyperlinkButton).Parent as StackPanel).Children[5] as HyperlinkButton).IsEnabled = true;

                (((sender as HyperlinkButton).Parent as StackPanel).Children[6] as HyperlinkButton).Content = "Spam";
                (((sender as HyperlinkButton).Parent as StackPanel).Children[6] as HyperlinkButton).IsEnabled = true;
            }
        }

        private async void RemoveUserPostButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement).DataContext is PostViewModel post)
            {
                await post.Post.DeleteAsync();

                (sender as HyperlinkButton).Content = "Deleted";
                (sender as HyperlinkButton).IsEnabled = false;
            }
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement).DataContext is PostViewModel post)
            {
                post.Post.Approve();

                (sender as HyperlinkButton).Content = "Approved";
                (sender as HyperlinkButton).IsEnabled = false;

                (((sender as HyperlinkButton).Parent as StackPanel).Children[4] as HyperlinkButton).Content = "Remove";
                (((sender as HyperlinkButton).Parent as StackPanel).Children[4] as HyperlinkButton).IsEnabled = true;

                (((sender as HyperlinkButton).Parent as StackPanel).Children[6] as HyperlinkButton).Content = "Spam";
                (((sender as HyperlinkButton).Parent as StackPanel).Children[6] as HyperlinkButton).IsEnabled = true;
            }
        }

        private async void SpamButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement).DataContext is PostViewModel post)
            {
                await post.Post.RemoveAsync(true);

                (sender as HyperlinkButton).Content = "Spammed";
                (sender as HyperlinkButton).IsEnabled = false;

                (((sender as HyperlinkButton).Parent as StackPanel).Children[4] as HyperlinkButton).Content = "Remove";
                (((sender as HyperlinkButton).Parent as StackPanel).Children[4] as HyperlinkButton).IsEnabled = true;

                (((sender as HyperlinkButton).Parent as StackPanel).Children[5] as HyperlinkButton).Content = "Approve";
                (((sender as HyperlinkButton).Parent as StackPanel).Children[5] as HyperlinkButton).IsEnabled = true;
            }
        }

        private async void PinButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement).DataContext is PostViewModel post)
            {
                // Retrieve first 3 posts of the subreddit, the first 2 are pinned (if not then there's at least one empty slot).
                var pinned = await Task.Run(() => App.RedditClient.Subreddit(name: post.Post.Subreddit).Posts.GetHot(limit: 3));
                int indexToInsert = 1;

                if (!pinned[0].Listing.Stickied || (pinned[0].Listing.Stickied && pinned[1].Listing.Stickied))
                {
                    indexToInsert = 1;
                }
                else if (!pinned[1].Listing.Stickied || pinned[0].Listing.Stickied)
                {
                    indexToInsert = 2;
                }

                // Sticky the post, finally.
                // This function has misleading documentation and I have no idea how to fix it, so the above
                // if statements are used here as a workaround.
                await post.Post.SetSubredditStickyAsync(indexToInsert, false);

                (sender as HyperlinkButton).Content = "Pinned";
                (sender as HyperlinkButton).IsEnabled = false;
            }
        }

        private void CopyLinkButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage package = new()
            {
                RequestedOperation = DataPackageOperation.Copy,
            };

            package.SetText("https://www.reddit.com" + ((sender as FrameworkElement).DataContext as PostViewModel).Post.Permalink);

            Clipboard.SetContent(package);
        }

        private async void CrossPostButton_Click(object sender, RoutedEventArgs e)
        {
            _ = await new CrossPostDialog(((sender as FrameworkElement).DataContext as PostViewModel).Post).ShowAsync();
        }

        private async void OnReportButtonClick(object sender, RoutedEventArgs e)
        {
            await new ReportDialog(((sender as FrameworkElement).DataContext as PostViewModel).Post).ShowAsync();
        }
    }
}