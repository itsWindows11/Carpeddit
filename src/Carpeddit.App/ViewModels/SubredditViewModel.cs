using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace Carpeddit.App.ViewModels
{
    public sealed partial class SubredditViewModel : ObservableObject
    {
        [ObservableProperty]
        private Subreddit subreddit;

        public string Title => string.IsNullOrWhiteSpace(Subreddit.Title) ? Subreddit.HeaderTitle : Subreddit.Title;

        public string FullName => Subreddit.Name;

        public string DisplayName => Subreddit.DisplayName;

        public string BannerUrl => Subreddit.BannerBackgroundImage;

        public bool IsTitleBlank => string.IsNullOrWhiteSpace(Subreddit.Title) || Subreddit.Title.Equals(Subreddit.DisplayNamePrefixed);

        public bool IsSubscribed
        {
            get => Subreddit.UserIsSubscriber ?? false;
            set
            {
                Subreddit.UserIsSubscriber = value;
                OnPropertyChanged();
            }
        }

        private readonly IRedditService service = Ioc.Default.GetService<IRedditService>();

        public SubredditViewModel(Subreddit subreddit)
        {
            Subreddit = subreddit;
        }

        [RelayCommand]
        public async Task JoinAsync()
        {
            try
            {
                var subreddits = new[] { FullName };

                if (IsSubscribed)
                {
                    await service.UnsubscribeFromSubredditsAsync(subreddits);
                    IsSubscribed = false;
                    return;
                }

                await service.SubscribeToSubredditsAsync(subreddits);
                IsSubscribed = true;
            }
            catch (Exception) { }
        }
    }
}
