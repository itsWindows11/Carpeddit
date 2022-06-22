using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.UI.Xaml.Media.Imaging;

namespace Carpeddit.App.Models
{
    public class PostViewModel : INotifyPropertyChanged
    {
        private Post _post;

        public Post Post
        {
            get => _post;
            set
            {
                _post = value;

                _upvoted = value.IsUpvoted;
                _downvoted = value.IsDownvoted;
            }
        }

        public string Title { get; set; }

        public string Author { get; set; }

        public DateTime Created { get; set; }

        public string Subreddit { get; set; }

        public string Description { get; set; }

        public bool ShouldDisplayUserFlair => !string.IsNullOrEmpty(Post.Listing.AuthorFlairText);

        public bool ShouldDisplayPostFlair => !string.IsNullOrEmpty(Post.Listing.LinkFlairText);

        public bool IsCurrentUserOP => App.RedditClient.Account.Me.Name == Author;

        public bool IsModDistinguished => Post.Listing.Distinguished == "moderator";

        public bool IsAdminDistinguished => Post.Listing.Distinguished == "admin";

        public string ShortDescription
        {
            get
            {
                if (Description.Length >= 350)
                {
                    return Description.Substring(0, 350) + "...";
                }

                return Description;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public Uri ImageUri
        {
            get
            {
                if (HasImage)
                {
                    return new Uri(Description, UriKind.Absolute);
                }
                return null;
            }
        }

        public int CommentsCount { get; set; }

        public string CommentsCountInUI => $"{CommentsCount} comment(s)";

        public Task<ObservableCollection<CommentViewModel>> GetCommentsAsync()
        {
            return Task.Run(() =>
            {
                ObservableCollection<CommentViewModel> comments = new();

                bool isCurrentUserMod = App.RedditClient.Subreddit(Post.Subreddit).About().SubredditData.UserIsModerator ?? false;

                foreach (Comment comment in Post.Comments.GetComments())
                {
                    CommentViewModel comment1 = new()
                    {
                        OriginalComment = comment,
                        IsTopLevel = true,
                        IsCurrentUserMod = isCurrentUserMod
                    };

                    _ = comment1.GetReplies(true, isCurrentUserMod);

                    comments.Add(comment1);
                }

                return comments;
            });
        }

        public bool HasImage => Uri.IsWellFormedUriString(Description, UriKind.Absolute) && (Description.StartsWith("https://", StringComparison.OrdinalIgnoreCase) || Description.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) && (Description.Contains("i.redd.it") || Description.Contains("i.imgur"));

        public MediaSource VideoSource
        {
            get
            {
                if (Post is LinkPost post && Uri.TryCreate(post.URL + "/DASHPlaylist.mpd", UriKind.Absolute, out Uri uri))
                {
                    return MediaSource.CreateFromUri(uri);
                }
                
                return null;
            }
        }

        private bool _upvoted;

        public bool Upvoted
        {
            get => _upvoted;
            set
            {
                _upvoted = value;

                OnPropertyChanged(nameof(Upvoted));
            }
        }

        private bool _downvoted;

        public bool Downvoted
        {
            get => _downvoted;
            set
            {
                _downvoted = value;

                OnPropertyChanged(nameof(Downvoted));
            }
        }

        public string VoteRatio
        {
            get
            {
                return FormatNumber(Post.UpVotes - Post.DownVotes);
            }
            private set
            {
                OnPropertyChanged(nameof(VoteRatio));
            }
        }

        public int RawVoteRatio
        {
            get
            {
                return Post.UpVotes - Post.DownVotes;
            }
            set
            {
                VoteRatio = FormatNumber(value);
                OnPropertyChanged(nameof(RawVoteRatio));
            }
        }

        public static string FormatNumber(long num)
        {
            if (num >= 100000000)
            {
                return (num / 1000000D).ToString("0.#M");
            }
            if (num >= 1000000)
            {
                return (num / 1000000D).ToString("0.##M");
            }
            if (num >= 100000)
            {
                return (num / 1000D).ToString("0.#k");
            }
            if (num >= 10000)
            {
                return (num / 1000D).ToString("0.##k");
            }

            return num.ToString("#,0");
        }
    }
}
