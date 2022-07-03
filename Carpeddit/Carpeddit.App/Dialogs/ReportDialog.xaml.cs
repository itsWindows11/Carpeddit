using Reddit.Controllers;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Dialogs
{
    public sealed partial class ReportDialog : ContentDialog
    {
        Post post;
        string reason;
        
        public ReportDialog(Post post)
        {
            InitializeComponent();

            this.post = post;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (post != null)
            {
                await post.ReportAsync(new Reddit.Inputs.LinksAndComments.LinksAndCommentsReportInput(reason: reason ?? ""));
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).IsChecked ?? false)
            {
                reason = (sender as RadioButton).Content.ToString();
            }
        }
    }
}
