using BracketPipe;
using Carpeddit.App.Collections;
using Carpeddit.App.Dialogs;
using Carpeddit.App.Models;
using Carpeddit.Common.Enums;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Reddit.Controllers;
using Reddit.Models;
using RtfPipe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
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

        public Subreddit Subreddit { get; set; }

        BulkConcurrentObservableCollection<CommentViewModel> commentsObservable = new();

        bool _isNotSeparate;

        Sort currentSort;
        
        bool initialCommentsLoaded;
        
        bool sortQueued;

        public PostDetailsPage()
        {
            InitializeComponent();

            Loaded += PostDetailsPage_Loaded;
        }

        private async void PostDetailsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isNotSeparate)
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
            }

            SortCombo.SelectionChanged += ComboBox_SelectionChanged;

            SecondPageFrame.Visibility = Visibility.Collapsed;
            MainGrid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);

            if (ActualWidth >= 750 && _isNotSeparate)
            {
                SecondPageFrame.Visibility = Visibility.Visible;
                MainGrid.ColumnDefinitions[1].Width = new GridLength(0.5, GridUnitType.Star);
            }

            try
            {
                SecondPageFrame.Navigate(typeof(SidebarPage), Subreddit, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
            }
            catch (COMException)
            {

            }

            CommentProgress.Visibility = Visibility.Visible;

            await foreach (var comment in Post.GetCommentsAsync(dummyParameter: null))
            {
                commentsObservable.Add(comment);
            }

            CommentsTree.ItemsSource = commentsObservable;
            CommentProgress.Visibility = Visibility.Collapsed;

            initialCommentsLoaded = true;

            if (sortQueued)
            {
                sortQueued = false;
                CommentProgress.Visibility = Visibility.Visible;
                CommentsTree.Visibility = Visibility.Collapsed;

                currentSort = SortCombo.SelectedItem as string switch
                {
                    "Best" => Sort.Best,
                    "New" => Sort.New,
                    "Rising" => Sort.Rising,
                    "Top" => Sort.Top,
                    "Controversial" => Sort.Controversial,
                    "Old" => Sort.Old,
                    "Random" => Sort.Random,
                    "QA" => Sort.QA,
                    _ => Sort.Hot,
                };

                if ((SortCombo.SelectedItem as string).Contains("Top"))
                {
                    currentSort = Sort.Top;
                }
                else if ((SortCombo.SelectedItem as string).Contains("Controversial"))
                {
                    currentSort = Sort.Controversial;
                }

                commentsObservable.Clear();

                commentsObservable.AddRange(await Post.GetCommentsAsync(sortType: currentSort.ToString().ToLower()));

                CommentProgress.Visibility = Visibility.Collapsed;
                CommentsTree.Visibility = Visibility.Visible;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Post = e.Parameter is PostViewModel ? e.Parameter as PostViewModel : (((PostViewModel, bool))e.Parameter).Item1;
            Subreddit = App.RedditClient.Subreddit(Post.Subreddit).About();

            _isNotSeparate = e.Parameter is (PostViewModel, bool) ? (((PostViewModel, bool))e.Parameter).Item2 : true;
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

            if (toggle.IsChecked ?? false)
            {
                await Post.Post.UpvoteAsync();
                Post.Upvoted = true;
                Post.Downvoted = false;
            }
            else
            {
                await Post.Post.UnvoteAsync();
                Post.Upvoted = false;
                Post.Downvoted = false;
            }
        }

        private async void DownvoteButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;

            if (toggle.IsChecked ?? false)
            {
                await Post.Post.DownvoteAsync();
                Post.Upvoted = false;
                Post.Downvoted = true;
            }
            else
            {
                await Post.Post.UnvoteAsync();
                Post.Upvoted = false;
                Post.Downvoted = false;
            }
        }

        private async void CommentUpvoteButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            CommentViewModel comment = toggle.Tag as CommentViewModel;

            if (toggle.IsChecked ?? false)
            {
                await comment.OriginalComment.UpvoteAsync();
                comment.Upvoted = true;
                comment.Downvoted = false;
            }
            else
            {
                await comment.OriginalComment.UnvoteAsync();
                comment.Upvoted = false;
                comment.Downvoted = false;
            }
        }

        private async void CommentDownvoteButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            CommentViewModel comment = toggle.Tag as CommentViewModel;

            if (toggle.IsChecked ?? false)
            {
                await comment.OriginalComment.DownvoteAsync();
                comment.Upvoted = false;
                comment.Downvoted = true;
            }
            else
            {
                await comment.OriginalComment.UnvoteAsync();
                comment.Upvoted = false;
                comment.Downvoted = false;
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
            string text = (sender.Inlines[0] as Windows.UI.Xaml.Documents.Run).Text;
            if (Window.Current.Content is Frame rootFrame)
            {
                rootFrame.Navigate(typeof(SubredditPage), App.RedditClient.Subreddit(text.Replace("r/", "")).About());
            }
        }

        private async void RemovePostButton_Click(object sender, RoutedEventArgs e)
        {
            await Post.Post.RemoveAsync();

            (sender as HyperlinkButton).Content = "Removed";
            (sender as HyperlinkButton).IsEnabled = false;

            (((sender as HyperlinkButton).Parent as StackPanel).Children[5] as HyperlinkButton).Content = "Approve";
            (((sender as HyperlinkButton).Parent as StackPanel).Children[5] as HyperlinkButton).IsEnabled = true;

            (((sender as HyperlinkButton).Parent as StackPanel).Children[6] as HyperlinkButton).Content = "Spam";
            (((sender as HyperlinkButton).Parent as StackPanel).Children[6] as HyperlinkButton).IsEnabled = true;
        }

        private async void RemoveUserPostButton_Click(object sender, RoutedEventArgs e)
        {
            await Post.Post.DeleteAsync();

            (sender as HyperlinkButton).Content = "Deleted";
            (sender as HyperlinkButton).IsEnabled = false;
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            Post.Post.Approve();

            (sender as HyperlinkButton).Content = "Approved";
            (sender as HyperlinkButton).IsEnabled = false;

            (((sender as HyperlinkButton).Parent as StackPanel).Children[4] as HyperlinkButton).Content = "Remove";
            (((sender as HyperlinkButton).Parent as StackPanel).Children[4] as HyperlinkButton).IsEnabled = true;

            (((sender as HyperlinkButton).Parent as StackPanel).Children[6] as HyperlinkButton).Content = "Spam";
            (((sender as HyperlinkButton).Parent as StackPanel).Children[6] as HyperlinkButton).IsEnabled = true;
        }

        private async void SpamButton_Click(object sender, RoutedEventArgs e)
        {
            await Post.Post.RemoveAsync(true);

            (sender as HyperlinkButton).Content = "Spammed";
            (sender as HyperlinkButton).IsEnabled = false;

            (((sender as HyperlinkButton).Parent as StackPanel).Children[4] as HyperlinkButton).Content = "Remove";
            (((sender as HyperlinkButton).Parent as StackPanel).Children[4] as HyperlinkButton).IsEnabled = true;

            (((sender as HyperlinkButton).Parent as StackPanel).Children[5] as HyperlinkButton).Content = "Approve";
            (((sender as HyperlinkButton).Parent as StackPanel).Children[5] as HyperlinkButton).IsEnabled = true;
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

        private async void DistinguishAsModerator_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement).DataContext is CommentViewModel comment)
            {
                _ = await comment.OriginalComment.DistinguishAsync("yes", false);

                // Refresh comments by setting items source
                // TreeView doesn't have a Refresh function so we have to do this.
                CommentsTree.ItemsSource = commentsObservable;
            }
        }

        private async void RemoveDistinguish_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement).DataContext is CommentViewModel comment)
            {
                _ = await comment.OriginalComment.DistinguishAsync("no", false);

                // Refresh comments by setting items source
                // TreeView doesn't have a Refresh function so we have to do this.
                CommentsTree.ItemsSource = commentsObservable;
            }
        }

        private void CopyLinkButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage package = new()
            {
                RequestedOperation = DataPackageOperation.Copy,
            };

            package.SetText("https://www.reddit.com" + ((sender as FrameworkElement).DataContext as CommentViewModel).OriginalComment.Permalink);

            Clipboard.SetContent(package);
        }

        private void CopyLinkButton1_Click(object sender, RoutedEventArgs e)
        {
            DataPackage package = new()
            {
                RequestedOperation = DataPackageOperation.Copy,
            };

            package.SetText("https://www.reddit.com" + Post.Post.Permalink);

            Clipboard.SetContent(package);
        }

        private async void LockCommentItem_Click(object sender, RoutedEventArgs e)
        {
            if (((sender as FrameworkElement).DataContext as CommentViewModel).OriginalComment.Listing.Locked)
            {
                await App.RedditClient.Account.Dispatch.LinksAndComments.UnlockAsync(((sender as FrameworkElement).DataContext as CommentViewModel).OriginalComment.Fullname);
            } else
            {
                await App.RedditClient.Account.Dispatch.LinksAndComments.LockAsync(((sender as FrameworkElement).DataContext as CommentViewModel).OriginalComment.Fullname);
            }
        }

        private async void ApproveCommentItem_Click(object sender, RoutedEventArgs e)
        {
            await App.RedditClient.Account.Dispatch.Moderation.ApproveAsync(((sender as FrameworkElement).DataContext as CommentViewModel).OriginalComment.Fullname);
        }

        private async void SpamComment_Click(object sender, RoutedEventArgs e)
        {
            await ((sender as FrameworkElement).DataContext as CommentViewModel).OriginalComment.RemoveAsync(true);
        }

        private async void CrossPostButton_Click(object sender, RoutedEventArgs e)
        {
            _ = await new CrossPostDialog(Post.Post).ShowAsync();
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (initialCommentsLoaded)
            {
                sortQueued = false;
                CommentProgress.Visibility = Visibility.Visible;
                CommentsTree.Visibility = Visibility.Collapsed;

                currentSort = e.AddedItems[0] as string switch
                {
                    "Best" => Sort.Best,
                    "New" => Sort.New,
                    "Rising" => Sort.Rising,
                    "Top" => Sort.Top,
                    "Controversial" => Sort.Controversial,
                    "Old" => Sort.Old,
                    "Random" => Sort.Random,
                    "QA" => Sort.QA,
                    _ => Sort.Hot,
                };

                if ((e.AddedItems[0] as string).Contains("Top"))
                {
                    currentSort = Sort.Top;
                }
                else if ((e.AddedItems[0] as string).Contains("Controversial"))
                {
                    currentSort = Sort.Controversial;
                }

                commentsObservable.Clear();

                commentsObservable.AddRange(await Post.GetCommentsAsync(sortType: currentSort.ToString().ToLower()));

                CommentProgress.Visibility = Visibility.Collapsed;
                CommentsTree.Visibility = Visibility.Visible;
            }
            else sortQueued = !initialCommentsLoaded;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            CommentsTree.Visibility = Visibility.Collapsed;
            CommentProgress.Visibility = Visibility.Visible;

            commentsObservable.Clear();

            commentsObservable.AddRange(await Post.GetCommentsAsync(sortType: currentSort.ToString().ToLower()));

            CommentsTree.ItemsSource = commentsObservable;
            CommentsTree.Visibility = Visibility.Visible;
            CommentProgress.Visibility = Visibility.Collapsed;
        }

        private async void OnReportButtonClick(object sender, RoutedEventArgs e)
        {
            _ = await new ReportDialog(Post.Post).ShowAsync();
        }
    }
}