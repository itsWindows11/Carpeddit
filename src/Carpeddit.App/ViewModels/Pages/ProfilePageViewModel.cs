using Carpeddit.Api.Enums;
using Carpeddit.Api.Models;
using Carpeddit.App.Collections;
using Carpeddit.App.Models;
using Carpeddit.App.Views;
using Carpeddit.Common.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.ViewModels.Pages
{
    public sealed partial class ProfilePageViewModel : ObservableObject
    {
        private User user;

        public User User
        {
            get => user;
            set
            {
                user = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsTitleBlank));

                source = new($"u/" + user.Name, sort: currentSort);
                Posts = new(source, 50, () =>
                {
                    if (loadedInitialPosts)
                        IsLoadingMore = true;
                    else
                        IsLoading = true;
                }, () =>
                {
                    loadedInitialPosts = true;
                    IsLoading = false;
                    IsLoadingMore = false;
                });
            }
        }

        public bool IsTitleBlank
        {
            get
            {
                if (User == null)
                    return true;

                return string.IsNullOrWhiteSpace(User.Subreddit.Title) || User.Subreddit.Title.Equals(User.Name);
            }
        }

        public BulkIncrementalLoadingCollection<PostLoadingSource, PostViewModel> Posts { get; private set; }

        private SortMode currentSort = SortMode.Best;
        private PostLoadingSource source;
        private bool loadedInitialPosts;

        [ObservableProperty]
        private bool isLoadingMore;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool infoLoaded;

        [RelayCommand]
        public Task SetSortAsync(SortMode mode)
        {
            currentSort = mode;
            source.CurrentSort = currentSort;

            loadedInitialPosts = false;

            return Posts.RefreshAsync();
        }

        [RelayCommand]
        public void SubredditClick(string subreddit)
            => WeakReferenceMessenger.Default.Send<MainFrameNavigationMessage>(new()
            {
                Page = typeof(SubredditInfoPage),
                Parameter = subreddit.Substring(2)
            });

        [RelayCommand]
        public void TitleClick(PostViewModel model)
            => ((Frame)Window.Current.Content).Navigate(typeof(PostDetailsPage), new PostDetailsNavigationInfo()
            {
                ShowFullPage = true,
                ItemData = model
            });
    }
}
