using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            TitleBar.SetAsTitleBar();
        }
    }
}
