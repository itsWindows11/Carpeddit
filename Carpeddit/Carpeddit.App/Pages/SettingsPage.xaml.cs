using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Carpeddit.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {

        public SettingsPage()
        {
            InitializeComponent();

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
    }
}
