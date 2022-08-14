using Windows.UI;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using System;
using Carpeddit.App.Helpers;
using System.Threading.Tasks;

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

        private async void SettingsPageLoaded(object sender, RoutedEventArgs e)
        {
            VersionTextBlock.Text = $"Carpeddit, version {string.Format("{0}.{1}.{2}.{3}", Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build, Package.Current.Id.Version.Revision)}";
            LoggingHelper.LogInfo("[SettingsPage] Loaded version info.");

            try
            {
                var prefs = await Task.Run(() => App.RedditClient.Account.Prefs());

                NSFWResultsToggleSwitch.IsOn = prefs.SearchIncludeOver18;
                CompactLinkToggleSwitch.IsOn = prefs.Compress;
                ClickTrackingToggleSwitch.IsOn = prefs.AllowClickTracking;

                LoggingHelper.LogInfo("[SettingsPage] Preferences loaded successfully.");
            } catch (Exception e1)
            {
                LoggingHelper.LogError("[SettingsPage] An error occurred while loading preferences.", e1);

                RedditPrefsExpander.IsEnabled = false;
            }

            NSFWResultsToggleSwitch.Toggled += NSFWResultsToggleSwitch_Toggled;
            CompactLinkToggleSwitch.Toggled += CompactLinkToggleSwitch_Toggled;
            ClickTrackingToggleSwitch.Toggled += ClickTrackingToggleSwitch_Toggled;
        }

        private async void NSFWResultsToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            try
            {
                var prefs = App.RedditClient.Account.Prefs();
                prefs.SearchIncludeOver18 = (sender as ToggleSwitch).IsOn;

                _ = await App.RedditClient.Account.UpdatePrefsAsync(new Reddit.Things.AccountPrefsSubmit(prefs, null, prefs.Beta, null));
            } catch
            {

            }
        }

        private async void CompactLinkToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            try
            {
                var prefs = App.RedditClient.Account.Prefs();
                prefs.Compress = (sender as ToggleSwitch).IsOn;

                _ = await App.RedditClient.Account.UpdatePrefsAsync(new Reddit.Things.AccountPrefsSubmit(prefs, null, prefs.Beta, null));
            } catch
            {

            }
        }

        private async void ClickTrackingToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            try
            {
                var prefs = App.RedditClient.Account.Prefs();
                prefs.AllowClickTracking = (sender as ToggleSwitch).IsOn;

                _ = await App.RedditClient.Account.UpdatePrefsAsync(new Reddit.Things.AccountPrefsSubmit(prefs, null, prefs.Beta, null));
            } catch
            {

            }
        }
    }
}
