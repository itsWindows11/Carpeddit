using Carpeddit.App.Models;
using Microsoft.Toolkit.Collections;
using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace Carpeddit.App.Helpers
{
    public static class PostHelpers
    {
        public static int SubtractVotes(int upvotes, int downvotes)
        {
            return upvotes - downvotes;
        }

        public static string SubtractVotesAndFormat(int upvotes, int downvotes)
        {
            int ratio = upvotes - downvotes;

            if (ratio >= 100000000)
            {
                return (ratio / 1000000D).ToString("0.#M");
            }
            if (ratio >= 1000000)
            {
                return (ratio / 1000000D).ToString("0.##M");
            }
            if (ratio >= 100000)
            {
                return (ratio / 1000D).ToString("0.#k");
            }
            if (ratio >= 10000)
            {
                return (ratio / 1000D).ToString("0.##k");
            }

            return ratio.ToString("#,0");
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

        public static string GetRelativeDate(DateTime time, bool approximate)
        {
            StringBuilder sb = new();

            string suffix = (time > DateTime.Now) ? " from now" : " ago";

            TimeSpan timeSpan = new(Math.Abs(DateTime.Now.Subtract(time).Ticks));

            if (timeSpan.Days > 0)
            {
                sb.AppendFormat("{0} {1}", timeSpan.Days,
                  (timeSpan.Days > 1) ? "days" : "day");
                if (approximate) return sb.ToString() + suffix;
            }
            if (timeSpan.Hours > 0)
            {
                sb.AppendFormat("{0}{1} {2}", (sb.Length > 0) ? ", " : string.Empty,
                  timeSpan.Hours, (timeSpan.Hours > 1) ? "hours" : "hour");
                if (approximate) return sb.ToString() + suffix;
            }
            if (timeSpan.Minutes > 0)
            {
                sb.AppendFormat("{0}{1} {2}", (sb.Length > 0) ? ", " : string.Empty,
                  timeSpan.Minutes, (timeSpan.Minutes > 1) ? "minutes" : "minute");
                if (approximate) return sb.ToString() + suffix;
            }
            if (timeSpan.Seconds > 0)
            {
                sb.AppendFormat("{0}{1} {2}", (sb.Length > 0) ? ", " : string.Empty,
                  timeSpan.Seconds, (timeSpan.Seconds > 1) ? "seconds" : "second");
                if (approximate) return sb.ToString() + suffix;
            }
            if (sb.Length == 0) return "right now";

            sb.Append(suffix);
            return sb.ToString();
        }

        public static string GetPostDescription(Post post)
        {
            if (post is SelfPost selfPost) return selfPost.SelfText;
            if (post is LinkPost linkPost) return linkPost.URL;
            return "";
        }

        public static Visibility GetDescVisibility(Post post)
        {
            if (string.IsNullOrWhiteSpace(GetPostDescription(post)))
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public static BitmapImage GetBitmapImage(Post post)
        {
            if (HasImage(post))
            {
                return new(new Uri(GetPostDescription(post), UriKind.Absolute));
            }
            return null;
        }

        public static bool HasImage(Post post)
        {
            if (Uri.IsWellFormedUriString(GetPostDescription(post), UriKind.Absolute) && (GetPostDescription(post).StartsWith("https://", StringComparison.OrdinalIgnoreCase) || GetPostDescription(post).StartsWith("http://", StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(GetPostDescription(post));
                    req.Method = "HEAD";
                    using var resp = req.GetResponse();
                    return resp.ContentType.ToLower(CultureInfo.InvariantCulture)
                               .StartsWith("image/", StringComparison.OrdinalIgnoreCase);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        public static Visibility HasImageUI(Post post)
        {
            if (Uri.IsWellFormedUriString(GetPostDescription(post), UriKind.Absolute) && (GetPostDescription(post).StartsWith("https://", StringComparison.OrdinalIgnoreCase) || GetPostDescription(post).StartsWith("http://", StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(GetPostDescription(post));
                    req.Method = "HEAD";
                    using var resp = req.GetResponse();
                    return resp.ContentType.ToLower(CultureInfo.InvariantCulture)
                               .StartsWith("image/", StringComparison.OrdinalIgnoreCase) ? Visibility.Visible : Visibility.Collapsed;
                }
                catch (Exception)
                {
                    return Visibility.Collapsed;
                }
            }
            return Visibility.Collapsed;
        }
    }


    public class PostLoadingHelper : IIncrementalSource<PostViewModel>
    {
        private readonly List<PostViewModel> posts;
        int postsCount = 0;

        public PostLoadingHelper()
        {
            posts = new();

            List<Post> frontpage = App.RedditClient.GetFrontPage(limit: 13);

            foreach (Post post in frontpage)
            {
                PostViewModel vm = new()
                {
                    Post = post,
                    Title = post.Title,
                    Description = GetPostDesc(post),
                    Created = post.Created,
                    Subreddit = post.Subreddit,
                    Author = post.Author
                };

                _ = vm.CommentsCount;

                posts.Add(vm);
            }

            for (postsCount = 0; postsCount < frontpage.Count; postsCount++);
        }

        public async Task<IEnumerable<PostViewModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            List<PostViewModel> posts1 = await Task.Run(async () =>
            {
                List<PostViewModel> posts = new();

                foreach (Post post in App.RedditClient.GetFrontPage(limit: pageSize, after: posts[postsCount - 1].Post.Fullname))
                {
                    PostViewModel vm = new()
                    {
                        Post = post,
                        Title = post.Title,
                        Description = GetPostDesc(post),
                        Created = post.Created,
                        Subreddit = post.Subreddit,
                        Author = post.Author
                    };

                    _ = vm.CommentsCount;

                    posts.Add(vm);
                }

                return posts;
            });

            return posts1.AsEnumerable();
        }

        private string GetPostDesc(Post post)
        {
            if (post is LinkPost linkPost)
            {
                return linkPost.URL;
            }
            else if (post is SelfPost selfPost)
            {
                return selfPost.SelfText;
            }

            return "No content";
        }
    }

}
