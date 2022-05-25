using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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

        private Task<List<Reddit.Things.Message>> GetInboxItemsAsync()
        {
            return Task.Run(() => App.RedditClient.Account.Messages.Inbox);
        }
    }
}
