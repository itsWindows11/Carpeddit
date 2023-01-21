using Carpeddit.App.ViewModels;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Views
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            Loaded += HomePage_Loaded;
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= HomePage_Loaded;

            MainList.ItemsSource = (await App.Client.GetFrontPageAsync()).Select(p => new PostViewModel()
            {
                Post = p
            });

            HomeRing.IsIndeterminate = false;
            HomeRing.Visibility = Visibility.Collapsed;
        }

        [RelayCommand]
        public void SubredditClick(string subreddit)
            => Frame.Navigate(typeof(SubredditInfoPage), subreddit.Substring(2));
    }
}
