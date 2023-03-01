using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using System;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace Carpeddit.App.ViewModels.Pages
{
    public sealed partial class SettingsPageViewModel : ObservableObject
    {
        public SettingsViewModel SettingsManager { get; } = Ioc.Default.GetService<SettingsViewModel>();

        [ObservableProperty]
        private RedditPrefsViewModel redditPrefs;

        [RelayCommand]
        public void ChangeAppTheme()
            => ((Frame)Window.Current.Content).RequestedTheme = SettingsManager.Theme;

        [RelayCommand]
        public void ChangeTintColor(Color color)
        {
            // Set color mode to "Custom color" for consistency.
            SettingsManager.TintType = 1;
            SettingsManager.TintColor = SettingsManager.TintColorsList.IndexOf(color);
        }

        [RelayCommand]
        public async Task PickTintFromFileAsync()
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

            SettingsManager.TintImageUri = newFile.Path;
        }

        [RelayCommand]
        public async Task PickTintFromUrlAsync(string url)
        {
            var textbox = new TextBox();

            var dialog = new ContentDialog()
            {
                Title = "Enter URL",
                Content = textbox,
                PrimaryButtonText = "OK",
                PrimaryButtonStyle = (Style)Application.Current.Resources["AccentButtonStyle"],
                SecondaryButtonText = "Cancel"
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && Uri.TryCreate(textbox.Text, UriKind.RelativeOrAbsolute, out _))
                SettingsManager.TintImageUri = textbox.Text;
        }

        [RelayCommand]
        public Task SaveRedditPrefsAsync()
            => RedditPrefs.UpdateAsync();

        [RelayCommand]
        public void UnsubscribeAllEmails()
        {
            RedditPrefs.EmailDigests = false;
            RedditPrefs.EmailPostReply = false;
            RedditPrefs.EmailChatRequest = false;
            RedditPrefs.EmailMessages = false;
            RedditPrefs.EmailCommunityDiscovery = false;
            RedditPrefs.EmailCommentReply = false;
        }
    }
}
