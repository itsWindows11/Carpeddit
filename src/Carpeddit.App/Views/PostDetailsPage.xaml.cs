using Carpeddit.App.Models;
using Carpeddit.App.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Carpeddit.App.Views
{
    public sealed partial class PostDetailsPage : Page
    {
        private PostViewModel ViewModel => DataContext as PostViewModel;

        private bool _showTitleBar;

        public PostDetailsPage()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            TitleBar.Loaded += (_, _1) => TitleBar.SetAsTitleBar();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            if (_showTitleBar)
            {
                TitleBar.Visibility = Visibility.Visible;
                TitleBar.SetAsTitleBar();
            } else
                TitleBar.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var navInfo = e.Parameter as PostDetailsNavigationInfo;
            DataContext = navInfo.ItemData;

            _showTitleBar = navInfo.ShowFullPage;
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            if (!Frame.CanGoBack)
                return;

            Frame.GoBack();
        }
    }
}
