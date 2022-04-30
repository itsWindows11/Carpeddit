using Windows.UI;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Carpeddit.App.Pages
{
    public sealed partial class SettingsPage : Page
    {

        public SettingsPage()
        {
            InitializeComponent();
            
            Loaded += SettingsPageLoaded;

            if (App.SViewModel.ColorMode == 2)
            {
                GridViewColorList.SelectedIndex = App.SViewModel.TintColor;
            }
        }

        private void Border_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Set color mode to "Custom color" for consistency.
            App.SViewModel.ColorMode = 2;
            var color = ((sender as Border).Background as SolidColorBrush).Color;
            GridViewColorList.SelectedIndex = App.SViewModel.TintColorsList.IndexOf(color);
            App.SViewModel.TintColor = App.SViewModel.TintColorsList.IndexOf(color);
        }

        private void GridViewColorList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Set color mode to "Custom color" for consistency.
            App.SViewModel.ColorMode = 2;
            var color = (Color)e.AddedItems[0];
            GridViewColorList.SelectedIndex = App.SViewModel.TintColorsList.IndexOf(color);
            App.SViewModel.TintColor = App.SViewModel.TintColorsList.IndexOf(color);
        }

        private void SettingsPageLoaded(object sender, RoutedEventArgs e)
        {
            VersionTextBlock.Text = $"Carpeddit, version {string.Format("{0}.{1}.{2}.{3}", Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build, Package.Current.Id.Version.Revision)}";
        }
    }
}
