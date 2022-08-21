using Carpeddit.App.Pages.Setup;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Carpeddit.App
{
    public sealed partial class FirstRunPage : Page
    {
        public FirstRunPage()
        {
            InitializeComponent();

            //Loaded += async (s, e) => await new FirstRunDialog().ShowAsync();

            ContentFrame.Navigate(typeof(WelcomePage), null, new SuppressNavigationTransitionInfo());
        }
    }
}
