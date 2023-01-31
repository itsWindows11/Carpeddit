using Carpeddit.Api.Enums;
using Carpeddit.Api.Models;
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
        public async Task<IList<Comment>> GetCommentsAsync(string postName, ListingInput input)
        {
            var response = await WebHelper.GetDeserializedResponseAsync<IList<Listing<IList<ApiObjectWithKind<Comment>>>>>($"/comments/{postName}");

            // First listing is always the post.
            response.RemoveAt(0);

            var commentsListing = response.FirstOrDefault();

            return commentsListing.Data.Children.Select(obj => obj.Data).ToList();
        }

        public async Task<IList<Post>> GetFrontpagePostsAsync(SortMode sort, ListingInput listingInput = null)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            if (listingInput != null)
            {
                queryString.Add("after", listingInput.After);
                queryString.Add("before", listingInput.Before);
                queryString.Add("limit", listingInput.Limit.ToString());
            }

            return (await WebHelper.GetDeserializedResponseAsync<Listing<IList<ApiObjectWithKind<Post>>>>("/.json?" + queryString.ToString()))
                .Data.Children.Select(p => p.Data).ToList();
        }

        public Task<User> GetMeAsync()
            => WebHelper.GetDeserializedResponseAsync<User>("/api/v1/me", true);

        public async Task<Subreddit> GetSubredditInfoAsync(string subreddit)
        {
            var response = await WebHelper.GetDeserializedResponseAsync<ApiObjectWithKind<Subreddit>>($"/r/{subreddit}/about.json");

            return response.Data;
        }

        public async Task<IList<Post>> GetSubredditPostsAsync(string subreddit, SortMode sort, ListingInput listingInput)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            if (listingInput != null)
            {
                queryString.Add("after", listingInput.After);
                queryString.Add("before", listingInput.Before);
                queryString.Add("limit", listingInput.Limit.ToString());
            }

            var listing = await WebHelper.GetDeserializedResponseAsync<Listing<IList<ApiObjectWithKind<Post>>>>($"/r/{subreddit}/{sort.ToString().ToLower()}.json?{queryString}");

            return listing.Data.Children.Select(p => p.Data).ToList();
        }

        public async Task<User> GetUserAsync(string userName)
        {
            return (await WebHelper.GetDeserializedResponseAsync<ApiObjectWithKind<User>>($"/user/{userName}/about.json")).Data;
        }

        public Task<UserKarmaContainer> GetUserKarmaAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<IList<Post>> GetUserPostsAsync(string user, SortMode sort, ListingInput listingInput)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            if (listingInput != null)
            {
                queryString.Add("after", listingInput.After);
                queryString.Add("before", listingInput.Before);
                queryString.Add("limit", listingInput.Limit.ToString());
            }

            return (await WebHelper.GetDeserializedResponseAsync<Listing<IList<ApiObjectWithKind<Post>>>>($"/user/{user}/submitted/{sort.ToString().ToLower()}.json?{queryString}"))
                .Data.Children.Select(p => p.Data).ToList();
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
