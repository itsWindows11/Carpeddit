using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using Windows.System;
using Windows.Media.Core;
using System.Collections.Generic;
using System.Linq;

namespace Carpeddit.App.ViewModels
{
    public sealed partial class PostViewModel : ObservableObject
    {
        private string description;
        private int? voteRatio;
        private bool? isUpvoted;
        private bool? isDownvoted;
        private bool? hasImage;
        private MediaSource videoSource;
        private List<Image> images;

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
        {
            get
            {
                if (!Post.IsSelf)
                    return Post.Url;

                return Post.Selftext.Length > 400 ? Post.Selftext.Substring(0, 400) + "..." : Post.Selftext;
            }
        }

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
                    // Do not directly change IsDownvoted property so that
                    // we don't end up downvoting everything we come across.
                    isDownvoted = false;
                    OnPropertyChanged(nameof(IsDownvoted));
                }

                _ = App.Services.GetService<IRedditService>().VoteAsync(new(value ? 1 : 0, Post.Name));
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
                    // Do not directly change IsUpvoted property so that
                    // we don't end up upvoting everything we come across.
                    isUpvoted = false;
                    OnPropertyChanged(nameof(IsUpvoted));
                }

                _ = App.Services.GetService<IRedditService>().VoteAsync(new(value ? -1 : 0, Post.Name));
            }
        }

        public Uri ImageUri
            => HasImage ? new Uri(Description.Trim(), UriKind.Absolute) : null;

        public bool IsGallery
            => Post.IsGallery ?? false;

        public bool HasImage
        {
            get
            {
                string description = Description.Trim();

                return hasImage ??= Uri.IsWellFormedUriString(description, UriKind.Absolute)
                    && (description.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                    || description.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                    && (description.Contains("preview.redd.it") || description.Contains("i.redd.it") || description.Contains("i.imgur"));
            }
        }

        public MediaSource VideoSource
        {
            get
            {
                if (!Post.IsSelf &&
                    Uri.TryCreate(Post.Url + "/HLSPlaylist.m3u8", UriKind.Absolute, out Uri uri))
                {
                    videoSource ??= MediaSource.CreateFromUri(uri);
                }

                return videoSource;
            }
        }

        public List<Image> Images
        {
            get
            {
                images ??= new();

                if (Post.MediaMetadata != null && !images.Any())
                {
                    foreach (var image in Post.MediaMetadata)
                    {
                        var image1 = image.Value.OriginalImage;

                        if (image1 != null && image1.Url != null)
                        {
                            image1.Url = image1.Url.Replace("preview.redd.it", "i.redd.it");

                            images.Add(image1);
                        }
                    }
                }

                return images;
            }
        }

        public async void OnMarkdownLinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out Uri link))
            {
                _ = await Launcher.LaunchUriAsync(link);
                return;
            }

            /*if (e.Link.StartsWith("/r/"))
            {
                (Window.Current.Content as Frame).Navigate(typeof(SubredditPage), App.RedditClient.Subreddit(name: e.Link.Substring(3)).About());
                return;
            }

            if (e.Link.StartsWith("r/"))
            {
                (Window.Current.Content as Frame).Navigate(typeof(SubredditPage), App.RedditClient.Subreddit(name: e.Link.Substring(2)).About());
                return;
            }

            if (e.Link.StartsWith("/u/"))
            {
                MainPage.Current.ContentFrame.Navigate(typeof(YourProfilePage), App.RedditClient.User(name: e.Link.Substring(3)).About());
                return;
            }

            if (e.Link.StartsWith("u/"))
            {
                MainPage.Current.ContentFrame.Navigate(typeof(YourProfilePage), App.RedditClient.User(name: e.Link.Substring(2)).About());
                return;
            }*/
        }
    }
}
