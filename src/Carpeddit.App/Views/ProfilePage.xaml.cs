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
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using Carpeddit.App.ViewModels.Pages;

namespace Carpeddit.App.Views
{
    public sealed partial class ProfilePage : Page
    {
        private IRedditService service = Ioc.Default.GetService<IRedditService>();

        public ProfilePageViewModel ViewModel { get; } = Ioc.Default.GetService<ProfilePageViewModel>();

        private CompositionPropertySet? _scrollerPropertySet;
        private Compositor? _compositor;
        private SpriteVisual? _backgroundVisual;
        private ScrollViewer? _scrollViewer;

        private object selectedSort;

        public ProfilePage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is User user)
            {
                ViewModel.User = user;
                ViewModel.InfoLoaded = true;
                Loaded += Page_Loaded;
            }
            else if (e.Parameter is string userName)
            {
                ViewModel.User = await service.GetUserAsync(userName);
                ViewModel.InfoLoaded = true;
                Page_Loaded(null, null);
                Bindings.Update();
            }
            else
            {
                ViewModel.User = await service.GetMeAsync();
                ViewModel.InfoLoaded = true;

                // Sometimes it may load too quickly so it might crash when finding a ScrollViewer
                await Task.Delay(150);

                Page_Loaded(null, null);
                Bindings.Update();
            }

            base.OnNavigatedTo(e);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = MainList.FindDescendant<ScrollViewer>();

            _scrollerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(_scrollViewer);
            _compositor = _scrollerPropertySet.Compositor;

            StartTitleFadeInAnimation(_compositor);

            ManipulationPropertySetReferenceNode scrollingProperties = _scrollerPropertySet.GetSpecializedReference<ManipulationPropertySetReferenceNode>();

            CreateImageBackgroundGradientVisual(scrollingProperties.Translation.Y, string.IsNullOrEmpty(ViewModel.User.Subreddit.BannerImage) ? null : new(WebUtility.HtmlDecode(ViewModel.User.Subreddit.BannerImage)));
        }

        private void OnCopyLinkFlyoutItemClick(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)e.OriginalSource).DataContext is not PostViewModel item)
                return;

            ViewModel.CopyLinkCommand?.Execute(item);
        }

        private void BackgroundHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_backgroundVisual == null) return;
            _backgroundVisual.Size = new Vector2((float)e.NewSize.Width, (float)e.NewSize.Height);
        }

        private void CreateImageBackgroundGradientVisual(ScalarNode scrollVerticalOffset, Uri uri)
        {
            if (string.IsNullOrEmpty(uri?.AbsoluteUri) || _compositor == null)
            {
                VisualStateManager.GoToState(this, "NoHeaderState", false);
                return;
            }

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

            visual.StartAnimation("Offset.Y", scrollVerticalOffset);
            imageBrush.StartAnimation("Offset.Y", -scrollVerticalOffset * 0.8f);

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

        private void StartTitleFadeInAnimation(Compositor compositor)
        {
            var animation = compositor.CreateScalarKeyFrameAnimation();

            animation.InsertKeyFrame(0, 0);
            animation.InsertKeyFrame(1, 1, compositor.CreateLinearEasingFunction());

            animation.Duration = TimeSpan.FromMilliseconds(600);

            var visual1 = SubredditFriendlyName.GetVisual();
            var visual2 = SubredditName.GetVisual();

            visual1.StartAnimation("Opacity", animation);
            visual2.StartAnimation("Opacity", animation);

            SubredditFriendlyName.Opacity = 1;
            SubredditName.Opacity = 1;
        }

        private async void Segmented_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is not FrameworkElement element)
                return;

            if (element.Tag is string tag && tag == "TopItem")
            {
                TopSortFlyout.ShowAt(TopItem);

                await Task.Delay(30);
                SortSegmented.SelectedIndex = -1;
            }
            else if (element.Tag is SortMode mode && (!selectedSort?.Equals(element.Tag) ?? false))
            {
                ViewModel.SetSortCommand?.Execute(mode);
            }

            selectedSort = element.Tag;
        }
    }
}
