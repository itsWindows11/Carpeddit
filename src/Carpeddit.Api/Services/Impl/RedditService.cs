using Carpeddit.Api.Enums;
using Carpeddit.Api.Helpers;
using Carpeddit.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Carpeddit.Api.Services
{
    // Implementation
    public sealed partial class RedditService : IRedditService
    {
        public Task<IList<Comment>> GetCommentsAsync(string postName, ListingInput input)
            => RunAsync<IList<Comment>>(async () =>
            {
                var response = await WebHelper.GetDeserializedResponseAsync<IList<Listing<IList<ApiObjectWithKind<Comment>>>>>($"/comments/{postName}?raw_json=1");

                // First listing is always the post.
                response.RemoveAt(0);

                var commentsListing = response.FirstOrDefault();

                return commentsListing.Data.Children.Select(obj => obj.Data).ToList();
            });

        public Task<IList<IPostReplyable>> GetCommentsOrMoreAsync(string postName, ListingInput input)
            => RunAsync<IList<IPostReplyable>>(async () =>
            {
                var response = await WebHelper.GetDeserializedResponseAsync<IList<Listing<IList<ApiObjectWithKind<object>>>>>($"/comments/{postName}?raw_json=1");

                // First listing is always the post.
                response.RemoveAt(0);

                return response.FirstOrDefault().Data.Children.Select<ApiObjectWithKind<object>, IPostReplyable>(obj =>
                {
                    if (obj.Kind == "more")
                        return JsonSerializer.Deserialize<More>(obj.Data.ToString());

                    return JsonSerializer.Deserialize<Comment>(obj.Data.ToString());
                }).ToList();
            });

        public Task<IList<Post>> GetFrontpagePostsAsync(SortMode sort, ListingInput listingInput = null)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            if (listingInput != null)
            {
                queryString.Add("after", listingInput.After);
                queryString.Add("before", listingInput.Before);
                queryString.Add("limit", listingInput.Limit.ToString());
            }

            return RunAsync<IList<Post>>(async () =>
            {
                var response = await WebHelper.GetDeserializedResponseAsync<Listing<IList<ApiObjectWithKind<Post>>>>("/.json?raw_json=1&" + queryString.ToString());
                return response.Data.Children.Select(p => p.Data).ToList();
            });
        }

        public Task<User> GetMeAsync()
            => RunAsync(() => WebHelper.GetDeserializedResponseAsync<User>("/api/v1/me?raw_json=1", true));

        public Task<Subreddit> GetSubredditInfoAsync(string subreddit)
            => RunAsync(async () => (await WebHelper.GetDeserializedResponseAsync<ApiObjectWithKind<Subreddit>>($"/r/{subreddit}/about.json?raw_json=1")).Data);

        public Task<IList<Post>> GetSubredditPostsAsync(string subreddit, SortMode sort, ListingInput listingInput)
            => RunAsync<IList<Post>>(async () =>
            {
                var queryString = HttpUtility.ParseQueryString(string.Empty);

                if (listingInput != null)
                {
                    queryString.Add("after", listingInput.After);
                    queryString.Add("before", listingInput.Before);
                    queryString.Add("limit", listingInput.Limit.ToString());
                }

                var listing = await WebHelper.GetDeserializedResponseAsync<Listing<IList<ApiObjectWithKind<Post>>>>($"/r/{subreddit}/{sort.ToString().ToLower()}.json?raw_json=1&{queryString}");

                return listing.Data.Children.Select(p => p.Data).ToList();
            });

        public Task<User> GetUserAsync(string userName)
            => RunAsync(async () => (await WebHelper.GetDeserializedResponseAsync<ApiObjectWithKind<User>>($"/user/{userName}/about.json?raw_json=1")).Data);

        public Task<UserKarmaContainer> GetUserKarmaAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IList<Post>> GetUserPostsAsync(string user, SortMode sort, ListingInput listingInput)
            => RunAsync<IList<Post>>(async () =>
            {
                var queryString = HttpUtility.ParseQueryString(string.Empty);

                if (listingInput != null)
                {
                    queryString.Add("after", listingInput.After);
                    queryString.Add("before", listingInput.Before);
                    queryString.Add("limit", listingInput.Limit.ToString());
                }

                var response = await WebHelper.GetDeserializedResponseAsync<Listing<IList<ApiObjectWithKind<Post>>>>($"/user/{user}/submitted/{sort.ToString().ToLower()}.json?raw_json=1&{queryString}");

                return response.Data.Children.Select(p => p.Data).ToList();
            });

        public Task VoteAsync(VotingInput input)
            => RunAsync(() =>
            {
                return WebHelper.PostAsync($"/api/vote", new Dictionary<string, string>()
                {
                    { "dir", input.Direction.ToString() },
                    { "id", input.Id }
                });
            });
    }

    // Helper methods
    public partial class RedditService
    {
        private async Task RunAsync(Func<Task> func)
        {
            int retries = 0;

            try
            {
                await func();
            }
            catch (UnauthorizedAccessException)
            {
                retries++;

                if (retries == 7)
                    throw;

                await TokenHelper.Instance.RefreshTokenAsync(AccountHelper.Instance.GetCurrentInfo().RefreshToken);
                await func();
            }
        }

        private async Task<T> RunAsync<T>(Func<Task<T>> func)
        {
            int retries = 0;

            try
            {
                return await func();
            }
            catch (UnauthorizedAccessException)
            {
                retries++;

                if (retries == 7)
                    throw;

                await TokenHelper.Instance.RefreshTokenAsync(AccountHelper.Instance.GetCurrentInfo().RefreshToken);
                return await func();
            }
        }
    }
}
