using Carpeddit.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Views
{
    public sealed partial class SettingsPage : Page
    {
        private SettingsViewModel SViewModel { get; } = App.Services.GetService<SettingsViewModel>();

        public SettingsPage()
        {
            InitializeComponent();
        }

        private void OnThemeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
            => ((Frame)Window.Current.Content).RequestedTheme = SViewModel.Theme;

        private void OnGridViewColorListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Set color mode to "Custom color" for consistency.
            SViewModel.TintType = 1;
            var color = (Color)e.AddedItems[0];
            SViewModel.TintColor = SViewModel.TintColorsList.IndexOf(color);
        }

        private void OnTintTypeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (SViewModel.TintType)
            {
                case 0:
                    _ = VisualStateManager.GoToState(this, "WindowTintNoneVisible", false);
                    SViewModel.TintImageUri = string.Empty;
                    SViewModel.TintColor = -1;
                    break;
                case 1:
                    _ = VisualStateManager.GoToState(this, "WindowTintColorVisible", false);
                    SViewModel.TintImageUri = string.Empty;
                    break;
                case 2:
                    _ = VisualStateManager.GoToState(this, "WindowTintImageVisible", false);
                    SViewModel.TintColor = -1;
                    break;
            }
        }

        private void OnImageCardClick(object sender, RoutedEventArgs e)
            => ImageCardMenuFlyout.ShowAt(e.OriginalSource as FrameworkElement);

        private async void OnImageCardFromFileClick(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".gif");

            var file = await picker.PickSingleFileAsync();

            if (file == null) return;

            var newFile = await file.CopyAsync(ApplicationData.Current.LocalCacheFolder, "bg.png", NameCollisionOption.ReplaceExisting);

            SViewModel.TintImageUri = newFile.Path;
        }

        private async void OnImageCardFromUrlClick(object sender, RoutedEventArgs e)
        {
            var textbox = new TextBox();

            var dialog = new ContentDialog()
            {
                Title = "Enter URL",
                Content = textbox,
                PrimaryButtonText = "OK",
                PrimaryButtonStyle = (Style)Resources["AccentButtonStyle"],
                SecondaryButtonText = "Cancel"
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && Uri.TryCreate(textbox.Text, UriKind.RelativeOrAbsolute, out _))
                SViewModel.TintImageUri = textbox.Text;
        }
    }
}
