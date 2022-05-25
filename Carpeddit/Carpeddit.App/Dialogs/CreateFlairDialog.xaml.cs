using Carpeddit.App.Pages;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Dialogs
{
    public sealed partial class CreateFlairDialog : ContentDialog
    {
        public bool IsUserFlair { get; set; }

        public CreateFlairDialog()
        {
            InitializeComponent();

            Loaded += CreateFlairDialog_Loaded;
        }

        private void CreateFlairDialog_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Loaded -= CreateFlairDialog_Loaded;

            TextColorToggle.Content = "Light";
            TextColorToggle.Checked += TextColorToggle_Checked;
            TextColorToggle.Unchecked += TextColorToggle_Unchecked;
        }

        private void TextColorToggle_Unchecked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            TextColorToggle.Content = "Light";
        }

        private void TextColorToggle_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            TextColorToggle.Content = "Dark";
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (IsUserFlair)
            {
                await ModToolsPage.Subreddit.Flairs.CreateUserFlairTemplateV2Async(NameText.Text, CanUserEditToggle.IsOn, (TextColorToggle.IsChecked ?? false) ? "dark" : "light", $"#{BackPicker.Color.ToString().Substring(3)}", ModOnlyToggle.IsOn);
            } else
            {
                await ModToolsPage.Subreddit.Flairs.CreateLinkFlairTemplateV2Async(NameText.Text, CanUserEditToggle.IsOn, (TextColorToggle.IsChecked ?? false) ? "dark" : "light", $"#{BackPicker.Color.ToString().Substring(3)}", ModOnlyToggle.IsOn);
            }

            Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }
    }
}
