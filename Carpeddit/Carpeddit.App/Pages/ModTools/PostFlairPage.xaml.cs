using Carpeddit.App.Dialogs;
using Reddit.Things;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Carpeddit.App.Pages.ModTools
{
    public sealed partial class PostFlairPage : Page
    {
        public PostFlairPage()
        {
            InitializeComponent();
        }

        private async void RemoveFlairButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement).Tag is FlairV2 flair)
            {
                ContentDialog dialog = new()
                {
                    Title = "Delete confirmation",
                    Content = $"Are you sure you want to delete the flair \"{flair.Text}\"?",
                    PrimaryButtonText = "Delete",
                    PrimaryButtonStyle = Resources["AccentButtonStyle"] as Style,
                    SecondaryButtonText = "Cancel"
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    await ModToolsPage.Subreddit.Flairs.DeleteFlairTemplateAsync(flair.Id);
                    PostFlairsList.ItemsSource = ModToolsPage.Subreddit.Flairs.GetLinkFlairV2();
                }
                else if (result == ContentDialogResult.Secondary)
                {
                    dialog.Hide();
                }
            }
        }

        private async void CreateFlairButton_Click(object sender, RoutedEventArgs e)
        {
            await new CreateFlairDialog()
            {
                IsUserFlair = false
            }.ShowAsync();
        }
    }
}
