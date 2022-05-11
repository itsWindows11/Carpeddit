using Carpeddit.App.Collections;
using Reddit.Things;
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
    public sealed partial class ModLogPage : Page
    {
        BulkConcurrentObservableCollection<ModActionChild> _modActions;

        public ModLogPage()
        {
            InitializeComponent();

            Loaded += ModLogPage_Loaded;
        }

        private async void ModLogPage_Loaded(object sender, RoutedEventArgs e)
        {
            Progress.Visibility = Visibility.Visible;

            _modActions = new();

            _modActions.AddRange(await Task.Run(() => ModToolsPage.Subreddit.GetLog().Data.Children));
            MainList.ItemsSource = _modActions;

            MainList.Visibility = Visibility.Visible;

            Progress.Visibility = Visibility.Collapsed;
        }

        private async void LoadMoreButton_Click(object sender, RoutedEventArgs e)
        {
            _modActions.AddRange(await Task.Run(() => ModToolsPage.Subreddit.GetLog(after: _modActions[_modActions.Count - 1].Data.TargetFullname).Data.Children));
        }
    }
}
