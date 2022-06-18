using BracketPipe;
using Carpeddit.App.Collections;
using Carpeddit.App.Models;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Reddit.Controllers;
using RtfPipe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
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
    public sealed partial class PostDetailsPage : Page
    {
        PostViewModel Post;

        BulkConcurrentObservableCollection<CommentViewModel> commentsObservable = new();

        public PostDetailsPage()
        {
            InitializeComponent();

            Loaded += PostDetailsPage_Loaded;
        }

        private void PostDetailsPage_Loaded(object sender, RoutedEventArgs e)
        {
            switch (App.SViewModel.ColorMode)
            {
                case 0:
                    ColorBrushBg.Color = Colors.Transparent;
                    break;
                case 1:
                    ColorBrushBg.Color = (Color)Resources["SystemAccentColor"];
                    break;
                case 2:
                    ColorBrushBg.Color = App.SViewModel.TintColorsList[App.SViewModel.TintColor];
                    break;
            }

            CommentProgress.Visibility = Visibility.Visible;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Post = e.Parameter as PostViewModel;

            commentsObservable.AddRange(await Post.GetCommentsAsync());

            CommentsTree.ItemsSource = commentsObservable;
            CommentProgress.Visibility = Visibility.Collapsed;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private async void AddCommentButton_Click(object sender, RoutedEventArgs e)
        {
            CommentEditBox.Document.GetText(TextGetOptions.FormatRtf, out string rtfText);
            
            Comment submittedComment = await Post.Post.Comment(RtfToMarkdown(rtfText)).SubmitAsync();

            commentsObservable.Add(new()
            {
                OriginalComment = submittedComment
            });

            CommentEditBox.Document.SetText(TextSetOptions.None, "");
        }

        private string RtfToMarkdown(string source)
        {
            using var w = new StringWriter();
            using var md = new MarkdownWriter(w);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Rtf.ToHtml(source, md);
            md.Flush();
            return w.ToString();
        }

        private async void RemoveCommentButton_Click(object sender, RoutedEventArgs e)
            => await ((e.OriginalSource as FrameworkElement).DataContext as CommentViewModel).OriginalComment.RemoveAsync();

        private async void DeleteCommentButton_Click(object sender, RoutedEventArgs e)
        {
            CommentViewModel comment = (e.OriginalSource as FrameworkElement).DataContext as CommentViewModel;

            // Delete the comment and remove it from the list.
            await comment.OriginalComment.DeleteAsync();

            if (comment.IsTopLevel)
            {
                commentsObservable.Remove(comment);
            } else
            {
                comment.ParentComment.Replies.Remove(comment);
            }
        }

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
            }
        }

        private void UserHyperlink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            string text = (sender.Inlines[1] as Windows.UI.Xaml.Documents.Run).Text;
            if (!text.Contains("[deleted]") && (Window.Current.Content as Frame).CanGoBack)
            {
                (Window.Current.Content as Frame).GoBack();
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

        private void ReplyButton_Click(object sender, RoutedEventArgs e)
        {
            ((e.OriginalSource as FrameworkElement).DataContext as CommentViewModel).ShowReplyUI = true;
        }

        private async void PinCommentButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement).DataContext is CommentViewModel comment && comment.IsTopLevel)
            {
                _ = await comment.OriginalComment.DistinguishAsync("yes", true);
                (sender as HyperlinkButton).Content = "Pinned";
                (sender as HyperlinkButton).IsEnabled = false;

                // Refresh comments by setting items source
                // TreeView doesn't have a Refresh function so we have to do this.
                CommentsTree.ItemsSource = commentsObservable;
            }
        }

        private async void ReplyCommentButton_Click(object sender, RoutedEventArgs e)
        {
            RichEditBox editBox = ((sender as Button).Parent as StackPanel).Children.First() as RichEditBox;
            editBox.Document.GetText(TextGetOptions.FormatRtf, out string rtfText);

            Comment submittedComment = await ((e.OriginalSource as FrameworkElement).DataContext as CommentViewModel).OriginalComment.ReplyAsync(RtfToMarkdown(rtfText));

            ((e.OriginalSource as FrameworkElement).DataContext as CommentViewModel).Replies.Add(new()
            {
                OriginalComment = submittedComment
            });

            ((e.OriginalSource as FrameworkElement).DataContext as CommentViewModel).ShowReplyUI = false;

            editBox.Document.SetText(TextSetOptions.None, "");
        }
    }
}