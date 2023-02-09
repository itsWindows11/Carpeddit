using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using Carpeddit.Common.Collections;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Views
{
    public sealed partial class MailboxPage : Page
    {
        private BulkObservableCollection<Message> Messages = new();

        public MailboxPage()
        {
            InitializeComponent();

            Loaded += OnMailboxPageLoaded;
        }

        private async void OnMailboxPageLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Loaded -= OnMailboxPageLoaded;

            Messages.AddRange(await App.Services.GetService<IRedditService>().GetMessagesAsync());

            LoadingRing.IsActive = false;
            LoadingRing.Visibility = Visibility.Collapsed;
        }
    }
}
