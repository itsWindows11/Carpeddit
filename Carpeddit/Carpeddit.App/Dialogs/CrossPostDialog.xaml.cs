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

namespace Carpeddit.App.Dialogs
{
    public sealed partial class CrossPostDialog : ContentDialog
    {
        private Post _post;

        public CrossPostDialog(Post post)
        {
            InitializeComponent();

            _post = post;
            Loaded += CrossPostDialog_Loaded;
        }

        private async void CrossPostDialog_Loaded(object sender, RoutedEventArgs e)
        {
            SubredditComboBox.ItemsSource = await Task.Run(() => App.RedditClient.Account.MySubscribedSubreddits(limit: 100));
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (SubredditComboBox.SelectedItem != null)
            {
                Subreddit subreddit = SubredditComboBox.SelectedItem as Subreddit;

                if (_post is SelfPost post)
                {
                    try
                    {
                        _ = await post.XPostToAsync(subreddit.Name);
                    } catch
                    {

                    }
                    return;
                }

                try
                {
                    _ = await (_post as LinkPost).XPostToAsync(subreddit.Name);
                }
                catch
                {

                }
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }
    }
}
