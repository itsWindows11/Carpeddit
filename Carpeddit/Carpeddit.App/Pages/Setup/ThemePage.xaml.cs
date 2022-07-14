using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Carpeddit.App.Pages.Setup
{
    public sealed partial class ThemePage : Page
    {
        public ThemePage()
        {
            InitializeComponent();
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
    }
}
