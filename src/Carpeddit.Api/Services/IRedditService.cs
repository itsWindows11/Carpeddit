﻿using Carpeddit.Api.Enums;
using Carpeddit.Api.Models;
using Carpeddit.App.Api.Models;
using Carpeddit.Models;
using Carpeddit.Models.Api;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carpeddit.Api.Services
{
    public interface IRedditService
    {
        /// <summary>
        /// Get a list of posts from a given subreddit.
        /// </summary>
        /// <param name="subreddit">The subreddit name.</param>
        Task<IList<Post>> GetSubredditPostsAsync(string subreddit, SortMode sort, ListingInput listingInput);

        /// <summary>
        /// Get a list of posts from a given subreddit.
        /// </summary>
        /// <param name="subreddit">The subreddit name.</param>
        Task<IList<Post>> GetUserPostsAsync(string user, SortMode sort, ListingInput listingInput);

        /// <summary>
        /// Get a list of posts from the frontpage.
        /// </summary>
        /// <param name="subreddit">The subreddit name.</param>
        Task<IList<Post>> GetFrontpagePostsAsync(SortMode sort, ListingInput listingInput);

        /// <summary>
        /// Gets a user using the specified username.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>The user.</returns>
        Task<User> GetUserAsync(string userName);

        /// <summary>
        /// Gets the currently authenticated user.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>The currently authenticated user.</returns>
        Task<User> GetMeAsync();

        /// <summary>
        /// Gets karma.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>Karma container.</returns>
        Task<UserKarmaContainer> GetUserKarmaAsync();

        /// <summary>
        /// Get a subreddit's info.
        /// </summary>
        /// <param name="subreddit">The subreddit name.</param>
        Task<Subreddit> GetSubredditInfoAsync(string subreddit);

        /// <summary>
        /// Get a post's comments.
        /// </summary>
        /// <param name="postName">The post name.</param>
        Task<IList<Comment>> GetCommentsAsync(string postName, ListingInput input);

        /// <summary>
        /// Votes on a thing.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="input">The query data to use.</param>
        /// <returns>The API response.</returns>
        Task VoteAsync(VotingInput input);
    }
}