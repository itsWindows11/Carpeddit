using Refit;
using System.Threading.Tasks;

namespace Carpeddit.App.Services
{
    public interface IRedditService
    {
        /// <summary>
        /// Get a list of posts from a given subreddit
        /// </summary>
        /// <param name="subreddit">The subreddit name.</param>
        [Get("/r/{subreddit}/.json")]
        Task<object> GetSubredditPostsAsync(string subreddit);
    }
}
