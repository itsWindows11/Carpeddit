using BracketPipe;
using Carpeddit.App.Collections;
using Carpeddit.App.Models;
using Reddit.Controllers;
using RtfPipe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    }
}