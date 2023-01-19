using Carpeddit.Api.Services;
using Carpeddit.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text.Json;
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
    }
}
