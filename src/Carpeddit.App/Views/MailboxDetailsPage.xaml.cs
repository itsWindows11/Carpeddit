using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using CommunityToolkit.Labs.WinUI.SizerBaseLocal;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Carpeddit.App.Views
{
    public sealed partial class MailboxDetailsPage : Page
    {
        private Message message;

        public MailboxDetailsPage()
        {
            InitializeComponent();

            Loaded += OnMailboxDetailsPageLoaded;
        }

        private void OnMailboxDetailsPageLoaded(object sender, RoutedEventArgs e)
        {
            MainList.Header = message;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            message = (Message)e.Parameter;
            base.OnNavigatedTo(e);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var richEditBox = MainList.FindDescendant<RichEditBox>();
            richEditBox.Document.GetText(TextGetOptions.None, out string text);

            await Ioc.Default.GetService<IRedditService>().CommentAsync("t4_" + message.Id, text);

            richEditBox.TextDocument.SetText(TextSetOptions.None, string.Empty);
        }
    }
}
