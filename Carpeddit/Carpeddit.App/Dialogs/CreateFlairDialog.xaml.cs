using Carpeddit.App.Pages;
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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Carpeddit.App.Dialogs
{
    public sealed partial class CreateFlairDialog : ContentDialog
    {
        public bool IsUserFlair { get; set; }

        public CreateFlairDialog()
        {
            InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (IsUserFlair)
            {
                ModToolsPage.Subreddit.Flairs.CreateUserFlairTemplateV2Async(NameText.Text, CanUserEditToggle.IsOn, TextPicker.Color.ToString(), BackPicker.Color.ToString(), ModOnlyToggle.IsOn);
            } else
            {
                ModToolsPage.Subreddit.Flairs.CreateLinkFlairTemplateV2Async(NameText.Text, CanUserEditToggle.IsOn, TextPicker.Color.ToString(), BackPicker.Color.ToString(), ModOnlyToggle.IsOn);
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }
    }
}
