using Carpeddit.App.ViewModels;
using Carpeddit.Common.Collections;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Views
{
    public sealed partial class HomePage : Page
    {
        private BulkObservableCollection<PostViewModel> _posts = new();

        public HomePage()
        {
            InitializeComponent();
            Loaded += HomePage_Loaded;
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= HomePage_Loaded;

            var posts = (await App.Client.GetFrontPageAsync()).Select(p => new PostViewModel()
            {
                Post = p
            });

            _posts.AddRange(posts);

            HomeRing.IsIndeterminate = false;
            HomeRing.Visibility = Visibility.Collapsed;
        }

        [RelayCommand]
        public void SubredditClick(string subreddit)
            => Frame.Navigate(typeof(SubredditInfoPage), subreddit.Substring(2));
    }
}
