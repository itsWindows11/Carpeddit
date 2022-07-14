using Carpeddit.App.Pages.Setup;
using Carpeddit.App.ViewModels;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Carpeddit.App.Dialogs
{
    public sealed partial class FirstRunDialog : ContentDialog
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        #region Setup Icons
        private readonly BitmapSource TermsImage =
            new BitmapImage(new Uri("ms-appx:///Assets/Setup/Terms.png"));

        private readonly BitmapSource PrivacyImage =
            new BitmapImage(new Uri("ms-appx:///Assets/Setup/Privacy.png"));

        private readonly BitmapSource AppearanceImage =
            new BitmapImage(new Uri("ms-appx:///Assets/Setup/Appearance.png"));

        private readonly BitmapSource DoneImage =
            new BitmapImage(new Uri("ms-appx:///Assets/Setup/Done.png"));
        #endregion

        public FirstRunDialog()
        {
            InitializeComponent();
            ContentDialog_SizeChanged(null, null);

            Navigate();

            App.SViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(App.SViewModel.TintColor) || e.PropertyName == nameof(App.SViewModel.ColorMode))
                {
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

                if (e.PropertyName == nameof(App.SViewModel.Theme))
                {
                    switch (App.SViewModel.Theme)
                    {
                        case 0:
                            RequestedTheme = ElementTheme.Light;
                            break;
                        case 1:
                            RequestedTheme = ElementTheme.Dark;
                            break;
                        case 2:
                            RequestedTheme = ElementTheme.Default;
                            break;
                    }
                }
            };
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SetupProgress > 0)
            {
                ViewModel.SetupProgress--;
            }

            Navigate();
        }

        private void PrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetupProgress++;
            Navigate();
        }
        
        private void Navigate()
        {
            BackButton.Visibility = ViewModel.SetupProgress > 0 ? Visibility.Visible : Visibility.Collapsed;

            ContentDialog_SizeChanged(null, null);

            switch (ViewModel.SetupProgress)
            {
                case 0:
                default:
                    Header.Text = "Welcome";
                    PrimaryButton.Content = "Next";
                    SetupInfo.Text = "Welcome";

                    SetupProgress.Value = 0;
                    SetupIcon.Source = TermsImage;
                    _ = SetupFrame.Navigate(typeof(WelcomePage));
                    break;

                case 1:
                    Header.Text = "Personalize";
                    PrimaryButton.Content = "Next";
                    SetupInfo.Text = "Step 1";

                    SetupProgress.Value = 40;
                    SetupIcon.Source = AppearanceImage;
                    _ = SetupFrame.Navigate(typeof(ThemePage));
                    break;

                case 2:
                    Header.Text = "Privacy";
                    PrimaryButton.Content = "Next";
                    SetupInfo.Text = "Step 2";

                    SetupProgress.Value = 80;
                    SetupIcon.Source = PrivacyImage;
                    _ = SetupFrame.Navigate(typeof(PrivacyPage));
                    break;

                case 3:
                    Header.Text = "All done!";
                    PrimaryButton.Content = "Finish";
                    SetupInfo.Text = "Done";

                    SetupProgress.Value = 100;
                    SetupIcon.Source = DoneImage;
                    _ = SetupFrame.Navigate(typeof(FinishPage));
                    break;

                case 4:
                    ViewModel.SetupProgress = 0;
                    Hide();

                    _ = (Window.Current.Content as Frame).Navigate(typeof(LoginPage));
                    break;
            }
        }

        private void ContentDialog_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double windowWidth = Window.Current.Bounds.Width;
            double windowHeight = Window.Current.Bounds.Height;

            if (windowWidth < 770)
            {
                SetupFrame.Width = windowWidth - 68;
                IconColumn.Width = new GridLength(0);
                ProgressColumn.Width = new GridLength(0);
                InfoGrid.ColumnSpacing = 0;
                ControlGrid.Margin = new Thickness(-32, -24, -24, -24);

                Header.Margin = BackButton.Visibility == Visibility.Visible ?
                    new Thickness(42, -5, 0, 0) : new Thickness(0, -5, 0, 0);
            }
            else
            {
                SetupFrame.Width = 770 - 284;
                IconColumn.Width = new GridLength(188);
                ProgressColumn.Width = new GridLength(210);
                InfoGrid.ColumnSpacing = 28;
                ControlGrid.Margin = new Thickness(-24);
                Header.Margin = new Thickness(0, -4, 0, 0);
            }

            RootGrid.Height = windowHeight < 498 ? windowHeight - 59 : 498 - 59;
        }
    }
}