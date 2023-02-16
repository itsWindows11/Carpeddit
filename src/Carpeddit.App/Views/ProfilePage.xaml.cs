using Carpeddit.Common.Helpers;
using Carpeddit.Models;
using Carpeddit.Models.Api;
using System.Net;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Carpeddit.Common.Collections;
using System.Linq;
using Carpeddit.App.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Carpeddit.App.Models;
using Carpeddit.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Carpeddit.Api.Enums;
using Carpeddit.Api.Models;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Microsoft.Toolkit.Uwp.UI;

namespace Carpeddit.App.Views
{
    public sealed partial class ProfilePage : Page
    {
        private User _user;
        private BulkObservableCollection<PostViewModel> _posts = new();
        private IRedditService service = App.Services.GetService<IRedditService>();
        private bool isLoadingMore;
        private bool _eventRegistered;

        private CompositionPropertySet? _scrollerPropertySet;
        private Compositor? _compositor;
        private SpriteVisual? _backgroundVisual;
        private ScrollViewer? _scrollViewer;

        public ProfilePage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is User user)
            {
                _user = user;
                Loaded += Page_Loaded;
                _eventRegistered = true;
            }
            else if (e.Parameter is string userName)
            {
                _user = await service.GetUserAsync(userName);
                Page_Loaded(null, null);
                Bindings.Update();
            }
            else
            {
                _user = await service.GetMeAsync();
                Page_Loaded(null, null);
                Bindings.Update();
            }

            base.OnNavigatedTo(e);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_eventRegistered)
                Loaded -= Page_Loaded;

            LoadingInfoRing.IsActive = false;
            LoadingInfoRing.Visibility = Visibility.Collapsed;

            if (_user.Subreddit.UserIsSubscriber)
                JoinButton.Content = "Unfollow";

            if (string.IsNullOrWhiteSpace(_user.Subreddit.Title))
                _ = VisualStateManager.GoToState(this, "NoDisplayName", false);

            _scrollViewer = MainList.FindDescendant<ScrollViewer>();

            _scrollerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(_scrollViewer);
            _compositor = _scrollerPropertySet.Compositor;

            ManipulationPropertySetReferenceNode scrollingProperties = _scrollerPropertySet.GetSpecializedReference<ManipulationPropertySetReferenceNode>();

            CreateImageBackgroundGradientVisual(scrollingProperties.Translation.Y, string.IsNullOrEmpty(_user.Subreddit.BannerImage) ? null : new(WebUtility.HtmlDecode(_user.Subreddit.BannerImage)));

            PostLoadingProgressRing.IsActive = true;
            PostLoadingProgressRing.Visibility = Visibility.Visible;

            var posts = (await service.GetUserPostsAsync(_user.Name, SortMode.New, new(limit: 50))).Select(p => new PostViewModel()
            {
                Post = p
            });

            _posts.AddRange(posts);

            MainList.ItemsSource = _posts;

            PostLoadingProgressRing.IsActive = false;
            PostLoadingProgressRing.Visibility = Visibility.Collapsed;

            var scrollViewer = ListHelpers.GetScrollViewer(MainList);

            scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;

            if (isLoadingMore || scrollViewer.VerticalOffset > scrollViewer.ScrollableHeight - 50)
                return;

            isLoadingMore = true;

            FooterProgress.Visibility = Visibility.Visible;

            var posts = (await service.GetUserPostsAsync(_user.Name, SortMode.New, new(after: _posts.Last().Post.Name, limit: 50))).Select(p => new PostViewModel()
            {
                Post = p
            });

            _posts.AddRange(posts);

            FooterProgress.Visibility = Visibility.Collapsed;

            isLoadingMore = false;
        }

        [RelayCommand]
        private void SubredditClick(string subreddit)
            => Frame.Navigate(typeof(SubredditInfoPage), subreddit.Substring(2));

        [RelayCommand]
        private void TitleClick(PostViewModel model)
            => ((Frame)Window.Current.Content).Navigate(typeof(PostDetailsPage), new PostDetailsNavigationInfo()
            {
                ShowFullPage = true,
                ItemData = model
            });

        private void OnCopyLinkFlyoutItemClick(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)e.OriginalSource).DataContext is not PostViewModel item)
                return;

            var package = new DataPackage()
            {
                RequestedOperation = DataPackageOperation.Copy,
            };

            package.SetText("https://www.reddit.com" + item.Post.Permalink);

            Clipboard.SetContent(package);
        }

        private void BackgroundHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_backgroundVisual == null) return;
            _backgroundVisual.Size = new Vector2((float)e.NewSize.Width, (float)e.NewSize.Height);
        }

        private void CreateImageBackgroundGradientVisual(ScalarNode scrollVerticalOffset, Uri uri)
        {
            if (string.IsNullOrEmpty(uri?.AbsoluteUri) || _compositor == null) return;

            var imageSurface = LoadedImageSurface.StartLoadFromUri(uri);
            imageSurface.LoadCompleted += OnImageSurfaceLoadCompleted;
            var imageBrush = _compositor.CreateSurfaceBrush(imageSurface);
            imageBrush.HorizontalAlignmentRatio = 0.5f;
            imageBrush.VerticalAlignmentRatio = 0.5f;
            imageBrush.Stretch = CompositionStretch.UniformToFill;

            var gradientBrush = _compositor.CreateLinearGradientBrush();
            gradientBrush.EndPoint = new Vector2(0, 1);
            gradientBrush.MappingMode = CompositionMappingMode.Relative;
            gradientBrush.ColorStops.Add(_compositor.CreateColorGradientStop(0.6f, Colors.White));
            gradientBrush.ColorStops.Add(_compositor.CreateColorGradientStop(1, Colors.Transparent));

            var maskBrush = _compositor.CreateMaskBrush();
            maskBrush.Source = imageBrush;
            maskBrush.Mask = gradientBrush;

            var visual = _backgroundVisual = _compositor.CreateSpriteVisual();
            visual.Size = new Vector2((float)BackgroundHost.ActualWidth, (float)BackgroundHost.Height);
            visual.Brush = maskBrush;

            gradientBrush.StartAnimation("Offset.Y", scrollVerticalOffset * 0.15f);

            ElementCompositionPreview.SetElementChildVisual(BackgroundHost, visual);
        }

        private void OnImageSurfaceLoadCompleted(LoadedImageSurface sender, LoadedImageSourceLoadCompletedEventArgs args)
        {
            sender.LoadCompleted -= OnImageSurfaceLoadCompleted;

            var animation = _compositor.CreateScalarKeyFrameAnimation();

            animation.InsertKeyFrame(0, 0);
            animation.InsertKeyFrame(1, 1, _compositor.CreateLinearEasingFunction());

            animation.Duration = TimeSpan.FromMilliseconds(600);
            _backgroundVisual.StartAnimation("Opacity", animation);
        }
    }
}
