using Carpeddit.Common.Enums;
using Reddit.Controllers;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Dialogs
{
    public sealed partial class CreatePostDialog : ContentDialog
    {
        private PostType _postType;
        private Subreddit _subreddit;
        private bool _fullyLoaded;

        public CreatePostDialog(PostType postType = PostType.Self, Subreddit subreddit = null)
        {
            InitializeComponent();

            _postType = postType;
            _subreddit = subreddit;

            Loaded += CreatePostDialog_Loaded;
        }

        private async void CreatePostDialog_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (_subreddit != null)
            {
                SubredditContainer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            
            SubredditComboBox.ItemsSource = await Task.Run(() => App.RedditClient.Account.MySubscribedSubreddits(limit: 100));
            _subreddit ??= SubredditComboBox.Items[0] as Subreddit;

            if (_subreddit.SubredditData.UserIsModerator ?? false)
            {
                DistingushCheckBox.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }

            _fullyLoaded = true;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();
            
            if (_fullyLoaded)
            {
                try
                {
                    switch (_postType)
                    {
                        case PostType.Self:
                            Post submittedPost = await _subreddit.SelfPost(title: TitleText.Text, selfText: ContentText.Text).SubmitAsync();

                            if (NSFWMarkCheck.IsChecked ?? false)
                                await submittedPost.MarkNSFWAsync();

                            if (DistingushCheckBox.IsChecked ?? false)
                                await submittedPost.DistinguishAsync("yes");
                            break;
                        case PostType.Link:
                            if (Uri.TryCreate(ContentText.Text, UriKind.Absolute, out _))
                            {
                                Post submittedPost1 = await _subreddit.LinkPost(title: TitleText.Text, url: ContentText.Text).SubmitAsync();
                                
                                if (NSFWMarkCheck.IsChecked ?? false)
                                    await submittedPost1.MarkNSFWAsync();

                                if (DistingushCheckBox.IsChecked ?? false)
                                    await submittedPost1.DistinguishAsync("yes");
                            }
                            break;
                    }
                    Hide();
                }
                catch
                {

                }
                deferral?.Complete();
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _postType = (sender as ComboBox).SelectedIndex == 0 ? PostType.Self : PostType.Link;
        }

        private void SubredditComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _subreddit = e.AddedItems[0] as Subreddit;
        }
    }
}
