using Reddit.Things;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Carpeddit.App.Pages
{
    public sealed partial class MailboxDetailsPage : Page
    {
        private Message Message;

        public MailboxDetailsPage()
        {
            InitializeComponent();

            switch (App.SViewModel.ColorMode)
            {
                case 0:
                    ColorBrushBg.Color = Colors.Transparent;
                    break;
                case 1:
                    ColorBrushBg.Color = (Color)Resources["SystemAccentColor"];
                    break;
                case 2:
                    ColorBrushBg.Color = App.SViewModel.TintColorsList[App.SViewModel.TintColor];
                    break;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Message message)
                Message = message;
        }
    }
}
