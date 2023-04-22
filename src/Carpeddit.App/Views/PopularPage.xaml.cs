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
    public sealed partial class PopularPage : Page, IRecipient<PostDetailsNavigationInfo>
    {
        public PopularPageViewModel ViewModel { get; } = Ioc.Default.GetService<PopularPageViewModel>();

        public PopularPage()
        {
            InitializeComponent();
            Loaded += OnPopularPageLoaded;
            Unloaded += OnPopularPageUnloaded;
        }

        private void OnPopularPageUnloaded(object sender, RoutedEventArgs e)
        {
            if (WeakReferenceMessenger.Default.IsRegistered<PostDetailsNavigationInfo>(this))
                WeakReferenceMessenger.Default.Unregister<PostDetailsNavigationInfo>(this);
        }

        private void OnPopularPageLoaded(object sender, RoutedEventArgs e)
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
