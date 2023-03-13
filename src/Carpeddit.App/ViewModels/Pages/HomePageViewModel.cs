using Carpeddit.Api.Enums;
using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using Carpeddit.App.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using System.Threading.Tasks;

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
            source = new();
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
    }
}
