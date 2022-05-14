using Carpeddit.App.Collections;
using Carpeddit.App.Models;
using Carpeddit.Common.Enums;
using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Carpeddit.App.Pages
{
    public sealed partial class SubredditPage : Page
    {
        public Subreddit Subreddit;
        BulkConcurrentObservableCollection<PostViewModel> posts = new();
        Sort currentSort;
        bool initialPostsLoaded;

        public SubredditPage()
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

            Loaded += SubredditPage_Loaded;
            SizeChanged += SubredditPage_SizeChanged;
        }

        private void SubredditPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width >= 750)
            {
                SubredditSidebarColumn.Width = new(0.4, GridUnitType.Star);
            }
            else if (e.NewSize.Width < 750)
            {
                SubredditSidebarColumn.Width = new(0);
            }
        }

        private async void SubredditPage_Loaded(object sender, RoutedEventArgs e)
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

            if (string.IsNullOrWhiteSpace(Subreddit.HeaderTitle) || Subreddit.HeaderTitle.Equals(Subreddit.Name))
            {
                SubredditFriendlyName.Visibility = Visibility.Collapsed;
                SubredditName.Style = Resources["SubtitleTextBlockStyle"] as Style;
                SubredditName.FontSize = 20;
            }

            ProgressR.Visibility = Visibility.Visible;

            try
            {
                SubredditHeaderImg.Source = new BitmapImage(new(Subreddit.SubredditData.BannerBackgroundImage.Replace("&amp;", "&")));
            }
            catch (UriFormatException)
            {

            }

            var modsList = Subreddit.GetModerators();

            RulesList.ItemsSource = Subreddit.GetRules().Rules;
            ModsList.ItemsSource = modsList;
            PostFlairsList.ItemsSource = Subreddit.Flairs.LinkFlairV2;

            await Task.Run(async () =>
            {
                foreach (var mod in modsList)
                {
                    if (mod.Name == App.RedditClient.Account.Me.Name)
                    {
                        Templates.PostTemplates.IsSubredditMod = true;

                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            ModerationToolsButton.Visibility = Visibility.Visible;
                        });
                        break;
                    }
                    else
                    {
                        Templates.PostTemplates.IsSubredditMod = false;
                    }
                }

                foreach (Subreddit subreddit in App.RedditClient.Account.MySubscribedSubreddits(limit: 100))
                {
                    if (subreddit.Name.Equals(Subreddit.Name))
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            JoinButton.Content = "Leave";
                        });
                        break;
                    }
                }
            });

            var posts1 = await Task.Run(async () =>
            {
                return await GetPostsAsync();
            });

            posts.AddRange(posts1);

            MainList.ItemsSource = posts;

            MainList.Visibility = Visibility.Visible;

            ProgressR.Visibility = Visibility.Collapsed;

            initialPostsLoaded = true;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Subreddit subreddit)
                Subreddit = subreddit;
            else throw new Exception("The parameter received must be a subreddit.");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private async Task<ObservableCollection<PostViewModel>> GetPostsAsync(string after = "", int limit = 24, string before = "", Sort sortType = Sort.Hot)
        {
            List<Post> frontpage = sortType switch
            {
                Sort.Best => Subreddit.Posts.GetBest(limit: limit, after: after, before: before),
                Sort.Controversial => Subreddit.Posts.GetControversial(limit: limit, after: after, before: before),
                Sort.New => Subreddit.Posts.GetNew(limit: limit, after: after, before: before),
                Sort.Rising => Subreddit.Posts.GetRising(limit: limit, after: after, before: before),
                _ => Subreddit.Posts.GetHot(limit: limit, after: after, before: before),
            };

            ObservableCollection<PostViewModel> postViews = new();

            foreach (Post post in frontpage)
            {
                PostViewModel vm = new()
                {
                    Post = post,
                    Title = post.Title,
                    Description = GetPostDesc(post),
                    Created = post.Created,
                    Subreddit = post.Subreddit,
                    Author = post.Author,
                    CommentsCount = post.Listing.NumComments
                };

                postViews.Add(vm);
            }

            return postViews;
        }

        private string GetPostDesc(Post post)
        {
            if (post is LinkPost linkPost)
            {
                return linkPost.URL;
            }
            else if (post is SelfPost selfPost)
            {
                return selfPost.SelfText;
            }

            return "No content";
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.Visibility = Visibility.Collapsed;
                FooterProgress.Visibility = Visibility.Visible;

                var posts1 = await Task.Run(async () =>
                {
                    return await GetPostsAsync(after: posts[posts.Count - 1].Post.Fullname);
                });

                posts.AddRange(posts1);

                button.Visibility = Visibility.Visible;
                FooterProgress.Visibility = Visibility.Collapsed;
            }
        }

        private async void JoinButton_Click(object sender, RoutedEventArgs e)
        {
            if ((JoinButton.Content as string).Equals("Leave"))
            {
                await Subreddit.UnsubscribeAsync();
                JoinButton.Content = "Join";
            }
            else
            {
                await Subreddit.SubscribeAsync();
                JoinButton.Content = "Leave";
            }
        }

        private void ModerationToolsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ModToolsPage), Subreddit);
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (initialPostsLoaded)
            {
                ProgressR.Visibility = Visibility.Visible;
                MainList.Visibility = Visibility.Collapsed;

                currentSort = e.AddedItems[0] as string switch
                {
                    "Best" => Sort.Best,
                    "Controversial" => Sort.Controversial,
                    "New" => Sort.New,
                    "Rising" => Sort.Rising,
                    _ => Sort.Hot,
                };

                posts.Clear();

                posts.AddRange(await Task.Run(async () =>
                {
                    return await GetPostsAsync(sortType: currentSort);
                }));

                ProgressR.Visibility = Visibility.Collapsed;
                MainList.Visibility = Visibility.Visible;
            }
        }
    }
}