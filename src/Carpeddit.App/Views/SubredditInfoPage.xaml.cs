using Carpeddit.Api.Enums;
using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using Carpeddit.App.ViewModels;
using Carpeddit.App.ViewModels.Pages;
using Carpeddit.Common.Collections;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using System;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Carpeddit.App.Views
{
    public sealed partial class SubredditInfoPage : Page
    {
        private IRedditService service = Ioc.Default.GetService<IRedditService>();

        public SubredditInfoPageViewModel ViewModel { get; } = Ioc.Default.GetService<SubredditInfoPageViewModel>();

        private CompositionPropertySet? _scrollerPropertySet;
        private Compositor? _compositor;
        private SpriteVisual? _backgroundVisual;
        private ScrollViewer? _scrollViewer;

        private object selectedSort;

        public SubredditInfoPage()
        {
            InitializeComponent();
        }

        private void SubredditInfoPage_Loaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = MainList.FindDescendant<ScrollViewer>();

            _scrollerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(_scrollViewer);
            _compositor = _scrollerPropertySet.Compositor;

            StartTitleFadeInAnimation(_compositor);

            ManipulationPropertySetReferenceNode scrollingProperties = _scrollerPropertySet.GetSpecializedReference<ManipulationPropertySetReferenceNode>();

            CreateImageBackgroundGradientVisual(scrollingProperties.Translation.Y, string.IsNullOrEmpty(ViewModel.Subreddit.BannerUrl) ? null : new(WebUtility.HtmlDecode(ViewModel.Subreddit.BannerUrl)));
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Subreddit subreddit)
            {
                ViewModel.Subreddit = new(subreddit);
                Loaded += SubredditInfoPage_Loaded;
            }
            else if (e.Parameter is string name)
            {
                ViewModel.Subreddit = new(await service.GetSubredditInfoAsync(name));
                ViewModel.InfoLoaded = true;
                SubredditInfoPage_Loaded(null, null);
                Bindings.Update();
            }
        }

        private void OnCopyLinkFlyoutItemClick(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)e.OriginalSource).DataContext is not PostViewModel item)
                return;

            ViewModel.CopyLinkCommand?.Execute(item);
        }

        private async void Segmented_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is not FrameworkElement element)
                return;

            if (element.Tag is string tag)
            {
                if (tag == "TopItem")
                    TopSortFlyout.ShowAt(TopItem);
                else if (tag == "ControversialItem")
                    ControversialSortFlyout.ShowAt(ControversialItem);

                await Task.Delay(30);
                SortSegmented.SelectedIndex = -1;
            } else if (element.Tag is SortMode mode && (!selectedSort?.Equals(element.Tag) ?? false))
            {
                ViewModel.SetSortCommand?.Execute(mode);
            }

            selectedSort = element.Tag;
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
    }
}
