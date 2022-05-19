using BracketPipe;
using Carpeddit.App.Collections;
using Carpeddit.App.Models;
using Reddit.Controllers;
using RtfPipe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Carpeddit.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PostDetailsPage : Page
    {
        PostViewModel Post;

        BulkConcurrentObservableCollection<CommentViewModel> commentsObservable = new();

        public PostDetailsPage()
        {
            InitializeComponent();

            var appViewTitleBar = ApplicationView.GetForCurrentView().TitleBar;

            appViewTitleBar.ButtonBackgroundColor = Colors.Transparent;
            appViewTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            Window.Current.SetTitleBar(AppTitleBar);

            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;
            Loaded += PostDetailsPage_Loaded;
        }

        private void PostDetailsPage_Loaded(object sender, RoutedEventArgs e)
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

            CommentProgress.Visibility = Visibility.Visible;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            AppTitleBar.Visibility = sender.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Post = e.Parameter as PostViewModel;

            commentsObservable.AddRange(await Post.GetCommentsAsync());

            CommentsTree.ItemsSource = commentsObservable;
            CommentProgress.Visibility = Visibility.Collapsed;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private async void AddCommentButton_Click(object sender, RoutedEventArgs e)
        {
            CommentEditBox.Document.GetText(TextGetOptions.FormatRtf, out string rtfText);
            
            Comment submittedComment = await Post.Post.Comment(RtfToMarkdown(rtfText)).SubmitAsync();

            commentsObservable.Add(new CommentViewModel()
            {
                OriginalComment = submittedComment
            });

            CommentEditBox.Document.SetText(TextSetOptions.None, "");
        }

        private string RtfToMarkdown(string source)
        {
            using var w = new StringWriter();
            using var md = new MarkdownWriter(w);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Rtf.ToHtml(source, md);
            md.Flush();
            return w.ToString();
        }
    }
}
