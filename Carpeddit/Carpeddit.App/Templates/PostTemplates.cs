using Carpeddit.App.Models;
using Carpeddit.App.Pages;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Carpeddit.App.Templates
{
    // Constructor
    public partial class PostTemplates
    {
        public PostTemplates()
        {
            InitializeComponent();
        }
    }

    // Fields and props
    public partial class PostTemplates
    {
        public static bool IsSubredditMod;
    }

    // Event handlers
    public partial class PostTemplates
    {
        private async void MarkdownLinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out Uri link))
            {
                await Launcher.LaunchUriAsync(link);
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
            if (!text.Contains("[deleted]"))
            {
                MainPage.Current.ContentFrame.Navigate(typeof(YourProfilePage), App.RedditClient.SearchUsers(new Reddit.Inputs.Search.SearchGetSearchInput(text)).FirstOrDefault(u => u.Name.Contains(text)));
            }
        }

        private void SubredditHyperlink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            string text = (sender.Inlines[1] as Windows.UI.Xaml.Documents.Run).Text;
            if (Window.Current.Content is Frame rootFrame)
            {
                rootFrame.Navigate(typeof(SubredditPage), App.RedditClient.SearchSubreddits(new Reddit.Inputs.Search.SearchGetSearchInput(text)).FirstOrDefault(s => s.Name.Contains(text)));
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

        private async void ApproveButton_Click(object sender, RoutedEventArgs e)
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
    }
}
