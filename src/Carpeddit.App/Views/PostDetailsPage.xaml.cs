using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using Carpeddit.App.Models;
using Carpeddit.App.ViewModels;
using Carpeddit.Common.Collections;
using Carpeddit.Models.Api;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Carpeddit.App.Views
{
    public sealed partial class PostDetailsPage : Page
    {
        private PostViewModel ViewModel => DataContext as PostViewModel;

        private bool _showTitleBar;

        private BulkObservableCollection<IPostReplyable> comments = new();

        public PostDetailsPage()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            TitleBar.Loaded += (_, _1) => TitleBar.SetAsTitleBar();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            if (_showTitleBar)
            {
                TitleBar.Visibility = Visibility.Visible;
                TitleBar.SetAsTitleBar();
            } else
                TitleBar.Visibility = Visibility.Collapsed;

            var comments = (await App.Services.GetService<IRedditService>()
                .GetCommentsOrMoreAsync(ViewModel.Post.Name.Substring(3), new ListingInput())).Select<IPostReplyable, IPostReplyable>(r =>
                {
                    if (r is Comment comment)
                    {
                        return new CommentViewModel
                        {
                            Comment = comment
                        };
                    }

                    return (More)r;
                });

            this.comments.AddRange(comments);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var navInfo = e.Parameter as PostDetailsNavigationInfo;
            DataContext = navInfo.ItemData;

            _showTitleBar = navInfo.ShowFullPage;
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            if (!Frame.CanGoBack)
                return;

            Frame.GoBack();
        }
    }

    public sealed class CommentItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CommentItemTemplate { get; set; }

        public DataTemplate MoreItemTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is CommentViewModel)
                return CommentItemTemplate;

            if (item is More)
                return MoreItemTemplate;

            return base.SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is Comment)
                return CommentItemTemplate;

            if (item is More)
                return MoreItemTemplate;

            return base.SelectTemplateCore(item, container);
        }
    }
}
