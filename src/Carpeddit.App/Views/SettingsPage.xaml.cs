using Carpeddit.App.ViewModels;
using Carpeddit.App.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Views
{
    public sealed partial class SettingsPage : Page
    {
        private SettingsPageViewModel ViewModel { get; } = App.Services.GetService<SettingsPageViewModel>();

        public SettingsPage()
        {
            InitializeComponent();
        }

        private void OnThemeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
            => ViewModel.ChangeAppThemeCommand?.Execute(null);

        private void OnGridViewColorListSelectionChanged(object sender, SelectionChangedEventArgs e)
            => ViewModel.ChangeTintColorCommand?.Execute(e.AddedItems[0]);

        private void OnTintTypeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ViewModel.SettingsManager.TintType)
            {
                case 0:
                    _ = VisualStateManager.GoToState(this, "WindowTintNoneVisible", false);
                    ViewModel.SettingsManager.TintImageUri = string.Empty;
                    ViewModel.SettingsManager.TintColor = -1;
                    break;
                case 1:
                    _ = VisualStateManager.GoToState(this, "WindowTintColorVisible", false);
                    ViewModel.SettingsManager.TintImageUri = string.Empty;
                    break;
                case 2:
                    _ = VisualStateManager.GoToState(this, "WindowTintImageVisible", false);
                    ViewModel.SettingsManager.TintColor = -1;
                    break;
            }
        }

        private void OnImageCardClick(object sender, RoutedEventArgs e)
            => ImageCardMenuFlyout.ShowAt(e.OriginalSource as FrameworkElement);

        private async void OnRedditPrefsCardLoaded(object sender, RoutedEventArgs e)
        {
            RedditPrefsCard.IsEnabled = false;
            ViewModel.RedditPrefs = await RedditPrefsViewModel.GetForCurrentUserAsync();
            RedditPrefsCard.IsEnabled = true;
            RedditPrefsLoadingProgressRing.IsActive = false;
        }
    }
}
