using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Pages
{
    public sealed partial class CollectionsPage : Page
    {
        public CollectionsPage()
        {
            InitializeComponent();
        }

        private void SavedPostsButton_Click(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(SavedPostsPage));
        }
    }
}
