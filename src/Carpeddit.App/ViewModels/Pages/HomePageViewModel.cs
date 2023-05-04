using Carpeddit.Api.Enums;
using Carpeddit.Api.Helpers;
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
    public sealed partial class HomePageViewModel : ObservableObject
    {
        public BulkIncrementalLoadingCollection<PostLoadingSource, PostViewModel> Posts { get; }

        private SortMode currentSort = SortMode.Best;
        private readonly PostLoadingSource source;
        private bool loadedInitialPosts;

        [ObservableProperty]
        private bool isLoadingMore;

        [ObservableProperty]
        private bool isLoading;

        public HomePageViewModel()
        {
            source = new(sort: currentSort);
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
        public void UserClick(string name)
        {
            if (name == "[deleted]")
                return;

            WeakReferenceMessenger.Default.Send<MainFrameNavigationMessage>(new()
            {
                Page = typeof(ProfilePage),
                Parameter = name
            });
        }

        [RelayCommand]
        public void TitleClick(PostViewModel model)
            => ((Frame)Window.Current.Content).Navigate(typeof(PostDetailsPage), new PostDetailsNavigationInfo()
            {
                ShowFullPage = true,
                ItemData = model
            });

        [RelayCommand]
        public void PostSelected(PostViewModel model)
        {
            if (model == null) return;

            WeakReferenceMessenger.Default.Send<PostDetailsNavigationInfo>(new()
            {
                ShowFullPage = false,
                ItemData = model
            });
        }

        [RelayCommand]
        public void SortSelectionChanged(string sort)
        {
            currentSort = StringToSortTypeConverter.ToSortMode(sort);
            SetSortCommand?.Execute(currentSort);
        }
    }
}
