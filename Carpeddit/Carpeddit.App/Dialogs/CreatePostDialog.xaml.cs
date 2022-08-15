using Carpeddit.Common.Enums;
using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Dialogs
{
    public sealed partial class CreatePostDialog : ContentDialog
    {
        private PostType _postType;
        private Subreddit _subreddit;
        private bool _fullyLoaded;
        private bool isCustomSubreddit;
        private List<string> subreddits;

        public CreatePostDialog(PostType postType = PostType.Self, Subreddit subreddit = null)
        {
            InitializeComponent();

            subreddits = new();
            _postType = postType;
            _subreddit = subreddit;

            Loaded += CreatePostDialog_Loaded;
        }

        private async void CreatePostDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (_subreddit != null)
            {
                SubredditContainer.Visibility = Visibility.Collapsed;
            }

            subreddits.AddRange(await Task.Run(() =>
            {
                var list = new List<string>();
                var subreddits = App.RedditClient.Account.MySubscribedSubreddits(limit: 40);

                list.Add("Other");

                foreach (Subreddit subreddit in subreddits)
                {
                    list.Add(subreddit.SubredditData.DisplayNamePrefixed);
                }

                return list;
            }));
            
            SubredditComboBox.ItemsSource = subreddits;
            _subreddit ??= App.RedditClient.Subreddit(subreddits[1]);

            if (_subreddit.SubredditData.UserIsModerator ?? false)
            {
                DistingushCheckBox.Visibility = Visibility.Visible;
            }

            _fullyLoaded = true;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();
            
            if (_fullyLoaded)
            {
                if (isCustomSubreddit)
                    _subreddit = App.RedditClient.Subreddit(SubredditTextBox.Text.Replace("r/", string.Empty));
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

            if (ContentText != null)
            {
                if (_postType == PostType.Link)
                    ContentText.Header = "Link";
                else
                    ContentText.Header = "Text";
            }
        }

        private void SubredditComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!e.AddedItems[0].Equals("Other"))
            {
                isCustomSubreddit = false;
                SubredditTextBox.Visibility = Visibility.Collapsed;
                _subreddit = App.RedditClient.Subreddit(e.AddedItems[0] as string);
            }
            else
            {
                SubredditTextBox.Visibility = Visibility.Visible;
                isCustomSubreddit = true;
            }
        }
    }
}
