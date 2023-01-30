using Carpeddit.Api.Enums;
using Carpeddit.App.Api.Models;
using Carpeddit.Models;
using Carpeddit.Models.Api;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Carpeddit.Api.Services
{
    public sealed class RedditService : IRedditService
    {
        public async Task<IEnumerable<Post>> GetFrontpagePostsAsync(SortMode sort, ListingInput listingInput = null)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            if (listingInput != null)
            {
                queryString.Add("after", listingInput.After);
                queryString.Add("before", listingInput.Before);
                queryString.Add("limit", listingInput.Limit.ToString());
            }

            return (await WebHelper.GetDeserializedResponseAsync<Listing<IEnumerable<ApiObjectWithKind<Post>>>>("/.json?" + queryString.ToString()))
                .Data.Children.Select(p => p.Data);
        }

        public Task<User> GetMeAsync()
            => WebHelper.GetDeserializedResponseAsync<User>("/api/v1/me", true);

        public async Task<Subreddit> GetSubredditInfoAsync(string subreddit)
        {
            var response = await WebHelper.GetDeserializedResponseAsync<ApiObjectWithKind<Subreddit>>($"/r/{subreddit}/about.json");

            return response.Data;
        }

        public async Task<IEnumerable<Post>> GetSubredditPostsAsync(string subreddit, SortMode sort, ListingInput listingInput)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            if (listingInput != null)
            {
                queryString.Add("after", listingInput.After);
                queryString.Add("before", listingInput.Before);
                queryString.Add("limit", listingInput.Limit.ToString());
            }

            var listing = await WebHelper.GetDeserializedResponseAsync<Listing<IEnumerable<ApiObjectWithKind<Post>>>>($"/r/{subreddit}/{sort.ToString().ToLower()}.json?{queryString}");

            return listing.Data.Children.Select(p => p.Data);
        }

        public async Task<User> GetUserAsync(string userName)
        {
            return (await WebHelper.GetDeserializedResponseAsync<ApiObjectWithKind<User>>($"/user/{userName}/about.json")).Data;
        }

        public Task<UserKarmaContainer> GetUserKarmaAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<Post>> GetUserPostsAsync(string user, SortMode sort, ListingInput listingInput)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            if (listingInput != null)
            {
                queryString.Add("after", listingInput.After);
                queryString.Add("before", listingInput.Before);
                queryString.Add("limit", listingInput.Limit.ToString());
            }

            return (await WebHelper.GetDeserializedResponseAsync<Listing<IEnumerable<ApiObjectWithKind<Post>>>>($"/user/{user}/submitted/{sort.ToString().ToLower()}.json?{queryString}"))
                .Data.Children.Select(p => p.Data);
        }

        public Task VoteAsync(VotingInput input)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            if (input != null)
            {
                queryString.Add("direction", input.Direction.ToString());
                queryString.Add("id", input.Id);
            }

            return WebHelper.PostAsync("/api/vote", new Dictionary<string, string>());
        }
    }
}
