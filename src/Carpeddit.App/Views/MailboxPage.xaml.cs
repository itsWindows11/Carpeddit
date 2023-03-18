using Carpeddit.App.ViewModels.Pages;
using CommunityToolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Views
{
    public sealed partial class MailboxPage : Page
    {
        public MailboxPageViewModel ViewModel { get; } = Ioc.Default.GetService<MailboxPageViewModel>();

        public MailboxPage()
        {
            InitializeComponent();
        }
    }
}
