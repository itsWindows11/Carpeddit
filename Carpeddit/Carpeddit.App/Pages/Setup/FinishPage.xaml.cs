using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Pages.Setup
{
    public sealed partial class FinishPage : Page
    {
        public FinishPage()
        {
            InitializeComponent();
        }

        private void OnPrimaryActionClick(object sender, RoutedEventArgs e)
        {
            App.SViewModel.SetupCompleted = true;
            (Window.Current.Content as Frame).Navigate(typeof(MainPage));
        }

        private async void OnSecondaryActionClick(object sender, RoutedEventArgs e)
        {
            App.SViewModel.SetupCompleted = true;
            if (!await ApplicationView.GetForCurrentView().TryConsolidateAsync())
            {
                Application.Current.Exit();
            }
        }
    }
}
