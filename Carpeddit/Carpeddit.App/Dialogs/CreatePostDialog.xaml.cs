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

        public CreatePostDialog(PostType postType = PostType.Self)
        {
            InitializeComponent();

            _postType = postType;

            Loaded += CreatePostDialog_Loaded;
        }

        private async void CreatePostDialog_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SubredditComboBox.ItemsSource = await Task.Run(() => App.RedditClient.Account.MySubscribedSubreddits(limit: 100));
            _subreddit = SubredditComboBox.Items[0] as Subreddit;

            _fullyLoaded = true;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (_fullyLoaded)
            {
                try
                {
                    switch (_postType)
                    {
                        case PostType.Self:
                            _ = await _subreddit.SelfPost(title: TitleText.Text, selfText: ContentText.Text).SubmitAsync();
                            break;
                        case PostType.Link:
                            if (Uri.TryCreate(ContentText.Text, UriKind.Absolute, out _))
                                _ = await _subreddit.LinkPost(title: TitleText.Text, url: ContentText.Text).SubmitAsync();
                            break;
                    }
                    Hide();
                }
                catch
                {

                }
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
