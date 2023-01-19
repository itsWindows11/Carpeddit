using Carpeddit.Models;
using Carpeddit.Models.Api;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carpeddit.Api.Services
{
    [Headers("User-Agent: Carpeddit")]
    public interface IRedditService
    {
        /// <summary>
        /// Get a list of posts from a given subreddit.
        /// </summary>
        /// <param name="subreddit">The subreddit name.</param>
        [Get("/r/{subreddit}/hot")]
        Task<IApiResponse<Listing<IEnumerable<ApiObjectWithKind<Post>>>>> GetSubredditPostsAsync(string subreddit, [Authorize] string accessToken);

        /// <summary>
        /// Get a list of posts from the frontpage.
        /// </summary>
        /// <param name="subreddit">The subreddit name.</param>
        [Get("/hot")]
        Task<IApiResponse<Listing<IEnumerable<ApiObjectWithKind<Post>>>>> GetFrontpagePostsAsync([Authorize] string accessToken);

        /// <summary>
        /// Gets the currently authenticated user.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>The currently authenticated user.</returns>
        [Get("/api/v1/me")]
        Task<IApiResponse<User>> GetCurrentlyAuthenticatedUserAsync([Authorize] string accessToken);

        /// <summary>
        /// Gets karma.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>Karma container.</returns>
        [Get("/api/v1/me/karma")]
        Task<IApiResponse<UserKarmaContainer>> GetUserKarmaAsync([Authorize] string accessToken);

        /*/// <summary>
        /// Gets karma.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>Karma container.</returns>
        [Get("/api/v1/me/prefs")]
        Task<AccountPrefs> GetPrefsAsync([Authorize] string accessToken);*/
    }
}
