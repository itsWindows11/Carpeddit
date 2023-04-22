using Carpeddit.App.Models;
using Carpeddit.App.ViewModels;
using Carpeddit.App.ViewModels.Pages;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Carpeddit.App.Views
{
    public sealed partial class HomePage : Page, IRecipient<PostDetailsNavigationInfo>
    {
        public HomePageViewModel ViewModel { get; } = Ioc.Default.GetService<HomePageViewModel>();

        public HomePage()
        {
            InitializeComponent();
            Loaded += OnHomePageLoaded;
            Unloaded += OnHomePageUnloaded;
        }

        private void OnHomePageUnloaded(object sender, RoutedEventArgs e)
        {
            if (WeakReferenceMessenger.Default.IsRegistered<PostDetailsNavigationInfo>(this))
                WeakReferenceMessenger.Default.Unregister<PostDetailsNavigationInfo>(this);
        }

        private void OnHomePageLoaded(object sender, RoutedEventArgs e)
        {
            if (!WeakReferenceMessenger.Default.IsRegistered<PostDetailsNavigationInfo>(this))
                WeakReferenceMessenger.Default.Register(this);
        }

        private void OnCopyLinkFlyoutItemClick(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)e.OriginalSource).DataContext is not PostViewModel item)
                return;

            ViewModel.CopyLinkCommand?.Execute(item);
        }

        public void Receive(PostDetailsNavigationInfo message)
            => PostDetailsFrame.Navigate(typeof(PostDetailsPage), message, new SuppressNavigationTransitionInfo());

        private void OnMainListSelectionChanged(object sender, SelectionChangedEventArgs e)
            => ViewModel.PostSelectedCommand?.Execute(MainList.SelectedItem);
    }
}
