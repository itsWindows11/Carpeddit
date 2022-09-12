using Carpeddit.App.Dialogs;
using Carpeddit.App.Models;
using Carpeddit.App.Pages;
using Carpeddit.App.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
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
                await post.UpvoteAsync();
            }
            else
            {
                await post.UnvoteAsync();
            }
        }

        private async void DownvoteButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            PostViewModel post = toggle.Tag as PostViewModel;

            if (toggle.IsChecked ?? false)
            {
                await post.DownvoteAsync();
            }
            else
            {
                await post.UnvoteAsync();
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
                await post.PinAsync();

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

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as HyperlinkButton;
            var post = button.DataContext as PostViewModel;

            if (post.Post.Listing.Saved)
            {
                button.Content = "Save";
                await post.Post.UnsaveAsync();
            } else
            {
                button.Content = "Unsave";
                await post.Post.SaveAsync(string.Empty);
            }
        }

        private async void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is ImageViewModel image)
            {
                _ = await new ImagePreviewDialog(image.Images).ShowAsync();
            } else
            {
                var post = (e.OriginalSource as FrameworkElement).DataContext as PostViewModel;
                _ = await new ImagePreviewDialog(post.ImageUri).ShowAsync();
            }
        }
    }
}
