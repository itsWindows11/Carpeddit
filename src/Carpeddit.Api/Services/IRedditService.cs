﻿using Carpeddit.Api.Enums;
using Carpeddit.Api.Models;
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
        /// Get a post's comments.
        /// </summary>
        /// <param name="postName">The post name.</param>
        Task<IList<IPostReplyable>> GetCommentsOrMoreAsync(string postName, ListingInput input);

        /// <summary>
        /// Votes on a thing.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="input">The query data to use.</param>
        /// <returns>A <see cref="Task" /> which represents the operation.</returns>
        Task VoteAsync(VotingInput input);

        /// <summary>
        /// Subscribes to a list of subreddits.
        /// </summary>
        /// <param name="subreddits">The list of subreddits to subscribe to.</param>
        /// <param name="srName">Whether the passed subreddit names are t5_ fullnames or subreddit display names.</param>
        /// <returns>A <see cref="Task" /> which represents the operation.</returns>
        Task SubscribeToSubredditsAsync(IEnumerable<string> subreddits, bool srName = false);

        /// <summary>
        /// Unsubscribes from a list of subreddits.
        /// </summary>
        /// <param name="subreddits">The list of subreddits to unsubscribe from.</param>
        /// <param name="srName">Whether the passed subreddit names are t5_ fullnames or subreddit display names.</param>
        /// <returns>A <see cref="Task" /> which represents the operation.</returns>
        Task UnsubscribeFromSubredditsAsync(IEnumerable<string> subreddits, bool srName = false);

        /// <summary>
        /// Gets the messages.
        /// </summary>
        /// <param name="type">The list type of the messages.</param>
        /// <returns>The list of messages.</returns>
        Task<IList<Message>> GetMessagesAsync(MessageListType type = MessageListType.Inbox);

        /// <summary>
        /// Comments on a link or message.
        /// </summary>
        /// <param name="fullname">The fullname of the comment, link or message.</param>
        /// <param name="rawMarkdownText">The text to submit.</param>
        /// <returns>A <see cref="Task" /> which represents the operation.</returns>
        Task CommentAsync(string fullname, string rawMarkdownText);

        /// <summary>
        /// Saves a link or a comment.
        /// </summary>
        /// <param name="fullname">The fullname of a comment or link.</param>
        /// <returns>A <see cref="Task" /> which represents the operation.</returns>
        Task SaveAsync(string fullname);

        /// <summary>
        /// Unsaves a link or a comment.
        /// </summary>
        /// <param name="fullname">The fullname of a comment or link.</param>
        /// <returns>A <see cref="Task" /> which represents the operation.</returns>
        Task UnsaveAsync(string fullname);
    }
}
