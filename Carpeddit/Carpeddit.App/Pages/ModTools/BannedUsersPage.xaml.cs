using Carpeddit.App.Collections;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Carpeddit.App.Pages.ModTools
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BannedUsersPage : Page
    {
        BulkConcurrentObservableCollection<Reddit.Controllers.Structures.BannedUser> _users;

        public BannedUsersPage()
        {
            InitializeComponent();

            _users = new();
            Loaded += MutedUsersPage_Loaded;
        }

        private async void MutedUsersPage_Loaded(object sender, RoutedEventArgs e)
        {
            Progress.Visibility = Visibility.Visible;
            MainList.Visibility = Visibility.Collapsed;

            _users.AddRange(await Task.Run(() => ModToolsPage.Subreddit.GetBannedUsers()));

            MainList.ItemsSource = _users;

            Progress.Visibility = Visibility.Collapsed;

            if (!_users.Any())
            {
                NoBannedItems.Visibility = Visibility.Visible;
            }
            else
            {
                MainList.Visibility = Visibility.Visible;
            }
        }

        private async void LoadMoreButton_Click(object sender, RoutedEventArgs e)
        {
            FooterProgress.Visibility = Visibility.Visible;
            LoadMoreButton.Visibility = Visibility.Collapsed;

            _users.AddRange(await Task.Run(() => ModToolsPage.Subreddit.GetBannedUsers(after: _users[_users.Count - 1].Id)));

            FooterProgress.Visibility = Visibility.Collapsed;
            LoadMoreButton.Visibility = Visibility.Visible;
        }
    }
}
