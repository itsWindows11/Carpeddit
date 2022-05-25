using Carpeddit.App.Collections;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Pages.ModTools
{
    public sealed partial class ModeratorsListPage : Page
    {
        BulkConcurrentObservableCollection<Reddit.Controllers.Structures.Moderator> _mods;

        public ModeratorsListPage()
        {
            InitializeComponent();

            _mods = new();
            Loaded += ModeratorsListPage_Loaded;
        }

        private async void ModeratorsListPage_Loaded(object sender, RoutedEventArgs e)
        {
            Progress.Visibility = Visibility.Visible;
            MainList.Visibility = Visibility.Collapsed;

            _mods.AddRange(await Task.Run(() => ModToolsPage.Subreddit.GetModerators()));

            MainList.ItemsSource = _mods;

            Progress.Visibility = Visibility.Collapsed;

            if (!_mods.Any())
            {
                NoModListItems.Visibility = Visibility.Visible;
            }
            else
            {
                MainList.Visibility = Visibility.Visible;
            }

            Progress.Visibility = Visibility.Collapsed;
            MainList.Visibility = Visibility.Visible;
        }

        private async void LoadMoreButton_Click(object sender, RoutedEventArgs e)
        {
            FooterProgress.Visibility = Visibility.Visible;
            LoadMoreButton.Visibility = Visibility.Collapsed;

            _mods.AddRange(await Task.Run(() => ModToolsPage.Subreddit.GetModerators(after: _mods[_mods.Count - 1].Id)));

            FooterProgress.Visibility = Visibility.Collapsed;
            LoadMoreButton.Visibility = Visibility.Visible;
        }
    }
}
