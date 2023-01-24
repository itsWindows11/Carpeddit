using Carpeddit.Api.Services;
using Carpeddit.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Carpeddit.App.ViewModels
{
    public sealed partial class PostViewModel : ObservableObject
    {
        private string description;
        private int? voteRatio;
        private bool? isUpvoted;
        private bool? isDownvoted;

        public Post Post { get; set; }

        public string Title
        {
            get => Post.Title;
            set
            {
                Post.Title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Description
            => description ??= Post.IsSelf ? Post.Selftext : Post.Url;

        public string ShortDescription
            => Description.Length > 400 ? Description.Substring(0, 400) + "..." : Description;

        public DateTime Created
        {
            get => Post.CreatedUtc.ToLocalTime();
            set
            {
                Post.CreatedUtc = value.ToUniversalTime();
                OnPropertyChanged(nameof(Created));
            }
        }

        public int VoteRatio
        {
            get => voteRatio ??= Post.Ups - Post.Downs;
            private set
            {
                voteRatio = value;
                OnPropertyChanged(nameof(VoteRatio));
            }
        }

        public bool IsUpvoted
        {
            get => isUpvoted ??= Post.Likes ?? false;
            set
            {
                isUpvoted = value;

                if (value)
                {
                    isDownvoted = false;
                    OnPropertyChanged(nameof(IsDownvoted));
                }

                _ = App.Services.GetService<IRedditService>().VoteAsync(new(value ? 1 : 0, Post.Name), App.Client.Info.AccessToken);
            }
        }

        public bool IsDownvoted
        {
            get => isDownvoted ??= !(Post.Likes ?? true);
            set
            {
                isDownvoted = value;

                if (value)
                {
                    isUpvoted = false;
                    OnPropertyChanged(nameof(IsUpvoted));
                }

                _ = App.Services.GetService<IRedditService>().VoteAsync(new(value ? -1 : 0, Post.Name), App.Client.Info.AccessToken);
            }
        }
    }
}
