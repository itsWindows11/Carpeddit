using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Carpeddit.App.Models
{
    public class PostViewModel
    {
        public Reddit.Controllers.Post Post { get; set; }

        public string Title
        {
            get => Post.Title;
        }

        public string Author
        {
            get => Post.Author;
        }

        public DateTime Created
        {
            get => Post.Created;
        }

        public string Subreddit
        {
            get => Post.Subreddit;
        }

        public string Description
        {
            get
            {
                if (Post is Reddit.Controllers.LinkPost linkPost)
                {
                    return linkPost.URL;
                }
                else if (Post is Reddit.Controllers.SelfPost selfPost)
                {
                    return selfPost.SelfText;
                }

                return "No content";
            }
        }

        public BitmapImage Image
        {
            get
            {
                if (HasImage)
                {
                    return new(new Uri(Description, UriKind.Absolute));
                }
                return null;
            }
        }

        public int CommentsCount => Post.Comments.Top.Count;

        public string CommentsCountInUI => $"{Post.Comments.Top.Count} comment(s)";

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

        public bool Upvoted
        {
            get
            {
                return Post.IsUpvoted && !Post.IsDownvoted;
            }
            set
            {
                if (value)
                {
                    Post.UpvoteAsync();
                    RawVoteRatio += 1;
                }
            }
        }

        public bool Downvoted
        {
            get
            {
                return Post.IsDownvoted && !Post.IsUpvoted;
            }
            set
            {
                if (value)
                {
                    Post.DownvoteAsync();
                    RawVoteRatio -= 1;
                }
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
