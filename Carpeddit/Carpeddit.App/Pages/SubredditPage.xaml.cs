using Carpeddit.App.Collections;
using Carpeddit.App.Dialogs;
using Carpeddit.App.Helpers;
using Carpeddit.App.Models;
using Carpeddit.App.ViewModels;
using Carpeddit.Common.Enums;
using Reddit.Controllers;
using Reddit.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Carpeddit.App.Pages
{
    public sealed partial class SubredditPage : Page
    {
        public Reddit.Controllers.Subreddit Subreddit;
        BulkConcurrentObservableCollection<PostViewModel> posts = new();
        Sort currentSort;
        SubSort currentSubSort;
        bool initialPostsLoaded;
        bool sortQueued;

        public SubredditPage()
        {
            InitializeComponent();

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
            SortCombo.SelectionChanged += ComboBox_SelectionChanged;

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

            LoggingHelper.LogInfo($"[SubredditPage] Loading moderators list in {Subreddit.SubredditData.DisplayNamePrefixed}...");

            var modsList = await Task.Run(() => Subreddit.GetModerators());

            LoggingHelper.LogInfo($"[SubredditPage] Loaded moderators list in {Subreddit.SubredditData.DisplayNamePrefixed}.");
            LoggingHelper.LogInfo($"[SubredditPage] Loading rules list in {Subreddit.SubredditData.DisplayNamePrefixed}...");

            var rulesList = await Task.Run(() => Subreddit.GetRules().Rules);

            LoggingHelper.LogInfo($"[SubredditPage] Loaded rules list in {Subreddit.SubredditData.DisplayNamePrefixed}.");

            RulesList.ItemsSource = rulesList;
            ModsList.ItemsSource = modsList;

            if (rulesList.Any())
            {
                RulesExpander.Visibility = Visibility.Visible;
            }

            if (modsList.Any())
            {
                ModeratorsExpander.Visibility = Visibility.Visible;
            }

            try
            {
                LoggingHelper.LogInfo($"[SubredditPage] Loading post flairs list in {Subreddit.SubredditData.DisplayNamePrefixed}...");

                var postFlairs = await Task.Run(() =>
                {
                    try
                    {
                        return Subreddit.Flairs.LinkFlairV2;
                    }
                    catch
                    {
                        return Enumerable.Empty<FlairV2>();
                    }
                });
                
                PostFlairsList.ItemsSource = postFlairs;

                if (postFlairs.Any())
                {
                    PostFlairsExpander.Visibility = Visibility.Visible;
                }

                LoggingHelper.LogInfo($"[SubredditPage] Loaded post flairs list in {Subreddit.SubredditData.DisplayNamePrefixed}.");
            }
            catch (Exception e1)
            {
                LoggingHelper.LogError($"[SubredditPage] An error occurred while loading post flairs list in {Subreddit.SubredditData.DisplayNamePrefixed}.", e1);
            }

            Templates.PostTemplates.IsSubredditMod = Subreddit.SubredditData.UserIsModerator ?? false;

            if (Subreddit.SubredditData.UserIsModerator ?? false)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ModerationToolsButton.Visibility = Visibility.Visible;
                });
            }

            if (Subreddit.SubredditData.UserIsSubscriber ?? false)
                JoinButton.Content = "Leave";

            LoggingHelper.LogInfo($"[SubredditPage] Loading posts in {Subreddit.SubredditData.DisplayNamePrefixed}...");

            posts.AddRange(await Task.Run(() => GetPosts()));

            MainList.ItemsSource = posts;

            LoggingHelper.LogInfo($"[SubredditPage] Loaded posts in {Subreddit.SubredditData.DisplayNamePrefixed}.");

            MainList.Visibility = Visibility.Visible;

            ProgressR.Visibility = Visibility.Collapsed;

            initialPostsLoaded = true;

            if (sortQueued)
            {
                LoggingHelper.LogInfo($"[SubredditPage] Using queued sort...");

                ProgressR.Visibility = Visibility.Visible;
                MainList.Visibility = Visibility.Collapsed;

                currentSort = SortCombo.SelectedItem as string switch
                {
                    "Best" => Sort.Best,
                    "New" => Sort.New,
                    "Rising" => Sort.Rising,
                    _ => Sort.Hot,
                };

                if ((SortCombo.SelectedItem as string).Contains("Top"))
                {
                    currentSort = Sort.Top;
                }
                else if ((SortCombo.SelectedItem as string).Contains("Controversial"))
                {
                    currentSort = Sort.Controversial;
                }

                currentSubSort = SortCombo.SelectedItem as string switch
                {
                    "Top (All)" => SubSort.TopAll,
                    "Top (Year)" => SubSort.TopYear,
                    "Top (Month)" => SubSort.TopMonth,
                    "Top (Week)" => SubSort.TopWeek,
                    "Top (Today)" => SubSort.TopToday,
                    "Top (Hour)" => SubSort.TopHour,
                    "Controversial (All)" => SubSort.ControversialAll,
                    "Controversial (Year)" => SubSort.ControversialYear,
                    "Controversial (Month)" => SubSort.ControversialMonth,
                    "Controversial (Week)" => SubSort.ControversialWeek,
                    "Controversial (Today)" => SubSort.ControversialToday,
                    "Controversial (Hour)" => SubSort.ControversialHour,
                    _ => SubSort.Default,
                };

                posts.Clear();

                posts.AddRange(await Task.Run(() => GetPosts(sortType: currentSort, t: currentSubSort.ToString().Replace("Top", string.Empty).Replace("Controversial", string.Empty).ToLower())));

                ProgressR.Visibility = Visibility.Collapsed;
                MainList.Visibility = Visibility.Visible;
                sortQueued = false;
            }
        }

        private IEnumerable<PostViewModel> GetPosts(string after = "", int limit = 100, string before = "", string t = "all", Sort sortType = Sort.Hot)
        {
            List<Reddit.Controllers.Post> frontpage = sortType switch
            {
                Sort.Best => Subreddit.Posts.GetBest(limit: limit, after: after, before: before),
                Sort.Controversial => Subreddit.Posts.GetControversial(limit: limit, after: after, before: before, t: t),
                Sort.New => Subreddit.Posts.GetNew(limit: limit, after: after, before: before),
                Sort.Rising => Subreddit.Posts.GetRising(limit: limit, after: after, before: before),
                Sort.Top => Subreddit.Posts.GetTop(limit: limit, after: after, before: before, t: t),
                _ => Subreddit.Posts.GetHot(limit: limit, after: after, before: before),
            };

            List<PostViewModel> postViews = new();

            foreach (Reddit.Controllers.Post post in frontpage)
            {
                PostViewModel vm = new()
                {
                    Post = post,
                    Title = post.Title,
                    Description = post.GetDescription(),
                    Created = post.Created,
                    Subreddit = post.Subreddit,
                    Author = post.Author,
                    CommentsCount = post.Listing.NumComments
                };

                postViews.Add(vm);
            }

            return postViews;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Reddit.Controllers.Subreddit subreddit)
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                SubredditSearchBox.IsEnabled = false;
                button.Visibility = Visibility.Collapsed;
                FooterProgress.Visibility = Visibility.Visible;
                SubredditSearchBox.IsEnabled = true;

                LoggingHelper.LogInfo($"[SubredditPage] Loading more posts in {Subreddit.SubredditData.DisplayNamePrefixed}...");

                await foreach (var post in PostHelpers.GetPostsAsync(Subreddit, after: posts.Last().Post.Fullname, sortType: currentSort, t: currentSubSort.ToString().Replace("Top", string.Empty).Replace("Controversial", string.Empty).ToLower()))
                {
                    posts.Add(post);
                }

                LoggingHelper.LogInfo($"[SubredditPage] Loaded more posts in {Subreddit.SubredditData.DisplayNamePrefixed}.");

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

                LoggingHelper.LogInfo($"[SubredditPage] Unsubscribed from {Subreddit.SubredditData.DisplayNamePrefixed}.");
            }
            else
            {
                await Subreddit.SubscribeAsync();
                JoinButton.Content = "Leave";

                LoggingHelper.LogInfo($"[SubredditPage] Subscribed into {Subreddit.SubredditData.DisplayNamePrefixed}.");
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
                sortQueued = false;
                LoggingHelper.LogInfo($"[SubredditPage] Using sort \"{e.AddedItems[0] as string}\" in {Subreddit.SubredditData.DisplayNamePrefixed}.");
                ProgressR.Visibility = Visibility.Visible;
                MainList.Visibility = Visibility.Collapsed;

                currentSort = e.AddedItems[0] as string switch
                {
                    "Best" => Sort.Best,
                    "New" => Sort.New,
                    "Rising" => Sort.Rising,
                    _ => Sort.Hot,
                };

                if ((e.AddedItems[0] as string).Contains("Top"))
                {
                    currentSort = Sort.Top;
                }
                else if ((e.AddedItems[0] as string).Contains("Controversial"))
                {
                    currentSort = Sort.Controversial;
                }

                currentSubSort = e.AddedItems[0] as string switch
                {
                    "Top (All)" => SubSort.TopAll,
                    "Top (Year)" => SubSort.TopYear,
                    "Top (Month)" => SubSort.TopMonth,
                    "Top (Week)" => SubSort.TopWeek,
                    "Top (Today)" => SubSort.TopToday,
                    "Top (Hour)" => SubSort.TopHour,
                    "Controversial (All)" => SubSort.ControversialAll,
                    "Controversial (Year)" => SubSort.ControversialYear,
                    "Controversial (Month)" => SubSort.ControversialMonth,
                    "Controversial (Week)" => SubSort.ControversialWeek,
                    "Controversial (Today)" => SubSort.ControversialToday,
                    "Controversial (Hour)" => SubSort.ControversialHour,
                    _ => SubSort.Default,
                };

                posts.Clear();

                LoggingHelper.LogInfo($"[SubredditPage] Loading posts using sort \"{e.AddedItems[0] as string}\" in {Subreddit.SubredditData.DisplayNamePrefixed}.");

                posts.AddRange(await Task.Run(() => GetPosts(sortType: currentSort, t: currentSubSort.ToString().Replace("Top", string.Empty).Replace("Controversial", string.Empty).ToLower())));

                ProgressR.Visibility = Visibility.Collapsed;
                MainList.Visibility = Visibility.Visible;
            } else sortQueued = !initialPostsLoaded;
        }

        private async void OnSearchBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            _ = await new SubredditSearchDialog(Subreddit.Name, sender.Text).ShowAsync();
        }

        private async void OnCreatePostButtonClick(object sender, RoutedEventArgs e)
        {
            _ = await new CreatePostDialog(subreddit: Subreddit).ShowAsync();
        }
    }
}