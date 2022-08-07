using Carpeddit.App.Collections;
using Carpeddit.App.Helpers;
using Carpeddit.App.Models;
using Reddit.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Pages
{
    public sealed partial class SavedPostsPage : Page
    {
        private BulkConcurrentObservableCollection<PostViewModel> savedPosts;

        public SavedPostsPage()
        {
            InitializeComponent();

            savedPosts = new();
            Loaded += SavedPostsPage_Loaded;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.Visibility = Visibility.Collapsed;
                FooterProgress.Visibility = Visibility.Visible;

                await foreach (var post in PostHelpers.GetUserPostHistoryAsync(App.RedditClient.Account.Me, after: savedPosts.Last().Post.Fullname, where: "saved"))
                {
                    savedPosts.Add(post);
                }

                button.Visibility = Visibility.Visible;
                FooterProgress.Visibility = Visibility.Collapsed;
            }
        }

        private async void SavedPostsPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadingProgress.Visibility = Visibility.Visible;

            await foreach (var post in PostHelpers.GetUserPostHistoryAsync(App.RedditClient.Account.Me, where: "saved"))
            {
                savedPosts.Add(post);
            }

            SavedPostsList.ItemsSource = savedPosts;

            LoadingProgress.Visibility = Visibility.Collapsed;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }
    }
}
