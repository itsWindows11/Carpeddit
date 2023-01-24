using Carpeddit.Api.Enums;
using Carpeddit.App.Api.Models;
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
        [Get("/r/{subreddit}/{sort}")]
        Task<IApiResponse<Listing<IEnumerable<ApiObjectWithKind<Post>>>>> GetSubredditPostsAsync(string subreddit, SortMode sort, [Authorize] string accessToken, [Query] ListingInput listingInput);

        /// <summary>
        /// Get a list of posts from a given subreddit.
        /// </summary>
        /// <param name="subreddit">The subreddit name.</param>
        [Get("/user/{user}/submitted/{sort}")]
        Task<IApiResponse<Listing<IEnumerable<ApiObjectWithKind<Post>>>>> GetUserPostsAsync(string user, SortMode sort, [Authorize] string accessToken, [Query] ListingInput listingInput);

        /// <summary>
        /// Get a list of posts from the frontpage.
        /// </summary>
        /// <param name="subreddit">The subreddit name.</param>
        [Get("/{sort}")]
        Task<IApiResponse<Listing<IEnumerable<ApiObjectWithKind<Post>>>>> GetFrontpagePostsAsync(SortMode sort, [Authorize] string accessToken, [Query] ListingInput listingInput);

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

        /// <summary>
        /// Get a list of posts from a given subreddit.
        /// </summary>
        /// <param name="subreddit">The subreddit name.</param>
        [Get("/r/{subreddit}/about")]
        Task<IApiResponse<ApiObjectWithKind<Subreddit>>> GetSubredditInfoAsync(string subreddit, [Authorize] string accessToken);
    }
}
