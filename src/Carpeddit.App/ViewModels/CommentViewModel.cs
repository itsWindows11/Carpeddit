using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using Windows.UI.Xaml;

namespace Carpeddit.App.ViewModels
{
    public sealed class CommentViewModel : ObservableObject, IPostReplyable
    {
        private int? voteRatio;
        private bool? isUpvoted;
        private bool? isDownvoted;
        private Thickness? commentMargin;

        public Comment Comment { get; set; }

        public string Body => Comment.Body;

        public DateTime Created
        {
            get => Comment.CreatedUtc.ToLocalTime();
            set
            {
                Comment.CreatedUtc = value.ToUniversalTime();
                OnPropertyChanged(nameof(Created));
            }
        }

        public int VoteRatio
        {
            get => voteRatio ??= Comment.Ups - Comment.Downs;
            private set
            {
                voteRatio = value;
                OnPropertyChanged(nameof(VoteRatio));
            }
        }

        public bool IsUpvoted
        {
            get => isUpvoted ??= Comment.Likes ?? false;
            set
            {
                isUpvoted = value;

                if (value)
                {
                    // Do not directly change IsDownvoted property so that
                    // we don't end up downvoting everything we come across.
                    isDownvoted = false;
                    OnPropertyChanged(nameof(IsDownvoted));
                }

                _ = App.Services.GetService<IRedditService>().VoteAsync(new(value ? 1 : 0, Comment.Name));
            }
        }

        public bool IsDownvoted
        {
            get => isDownvoted ??= !(Comment.Likes ?? true);
            set
            {
                isDownvoted = value;

                if (value)
                {
                    // Do not directly change IsUpvoted property so that
                    // we don't end up upvoting everything we come across.
                    isUpvoted = false;
                    OnPropertyChanged(nameof(IsUpvoted));
                }

                _ = App.Services.GetService<IRedditService>().VoteAsync(new(value ? -1 : 0, Comment.Name));
            }
        }

        public int Depth => Comment.Depth ?? 0;

        public Thickness CommentMargin => commentMargin ??= new((Depth * 8) - 4, 0, 0, 4);
    }
}
