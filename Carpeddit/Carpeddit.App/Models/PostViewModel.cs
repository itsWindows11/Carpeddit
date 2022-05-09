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

        public bool ShouldDisplayUserFlair
        {
            get => !string.IsNullOrEmpty(Post.Listing.AuthorFlairText);
        }

        public bool ShouldDisplayPostFlair
        {
            get => !string.IsNullOrEmpty(Post.Listing.LinkFlairText);
        }

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
            return Task.Run(async () =>
            {
                ObservableCollection<CommentViewModel> comments = new();

                foreach (Comment comment in Post.Comments.GetComments(limit: 100))
                {
                    CommentViewModel comment1 = new()
                    {
                        OriginalComment = comment
                    };

                    _ = await comment1.GetRepliesAsync(true);

                    comments.Add(comment1);
                }

                return comments;
            });
        }

        public bool HasImage
        {
            get
            {
                if (Uri.IsWellFormedUriString(Description, UriKind.Absolute) && (Description.StartsWith("https://", StringComparison.OrdinalIgnoreCase) || Description.StartsWith("http://", StringComparison.OrdinalIgnoreCase)))
                {
                    try
                    {
                        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Description);
                        req.Method = "HEAD";
                        using var resp = req.GetResponse();
                        return resp.ContentType.ToLower(CultureInfo.InvariantCulture)
                                   .StartsWith("image/", StringComparison.OrdinalIgnoreCase);
                    }
                    catch (Exception)
                    {

                    }
                }
                return false;
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
