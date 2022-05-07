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
    public partial class PostTemplates
    {
        public PostTemplates()
        {
            InitializeComponent();
        }
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
    }
}
