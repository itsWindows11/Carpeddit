using Reddit.Things;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Carpeddit.App.Pages
{
    public sealed partial class MailboxPage : Page
    {
        public MailboxPage()
        {
            InitializeComponent();

            Loaded += MailboxPage_Loaded;
        }

        private async void MailboxPage_Loaded(object sender, RoutedEventArgs e)
        {
            Progress.Visibility = Visibility.Visible;
            MainList.ItemsSource = await GetInboxItemsAsync();
            Progress.Visibility = Visibility.Collapsed;
        }

        private Task<List<Message>> GetInboxItemsAsync()
        {
            return Task.Run(() => App.RedditClient.Account.Messages.Inbox);
        }

        private void TextBlock_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is Message message)
            {
                (Window.Current.Content as Frame).Navigate(typeof(MailboxDetailsPage), message);
            }
        }

        private void UserHyperlink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            string text = (sender.Inlines[1] as Windows.UI.Xaml.Documents.Run).Text;
            if (!text.Contains("[deleted]"))
            {
                MainPage.Current.ContentFrame.Navigate(typeof(YourProfilePage), App.RedditClient.User(text).About());
            }
        }
    }
}
