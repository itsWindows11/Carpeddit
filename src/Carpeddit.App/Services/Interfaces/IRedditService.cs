using Carpeddit.Models;
using Refit;
using System.Threading.Tasks;

namespace Carpeddit.App.Services
{
    [Headers("User-Agent: Carpeddit")]
    public interface IRedditService
    {
        /// <summary>
        /// Get a list of posts from a given subreddit
        /// </summary>
        /// <param name="subreddit">The subreddit name.</param>
        [Get("/r/{subreddit}/.json")]
        Task<object> GetSubredditPostsAsync(string subreddit);

        /// <summary>
        /// Gets the currently authenticated user.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>The currently authenticated user.</returns>
        [Get("/api/v1/me")]
        Task<User> GetCurrentlyAuthenticatedUserAsync([Authorize] string accessToken);
    }
}
