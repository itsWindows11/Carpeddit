using Carpeddit.Api.Enums;
using Carpeddit.Api.Helpers;
using Carpeddit.Api.Models;
using Carpeddit.App.Api.Helpers;
using Carpeddit.App.Api.Models;
using Carpeddit.Models;
using Carpeddit.Models.Api;
using System;
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
            try
            {
                var response = await WebHelper.GetDeserializedResponseAsync<IList<Listing<IList<ApiObjectWithKind<Comment>>>>>($"/comments/{postName}");

                // First listing is always the post.
                response.RemoveAt(0);

                var commentsListing = response.FirstOrDefault();

                return commentsListing.Data.Children.Select(obj => obj.Data).ToList();
            } catch (UnauthorizedAccessException)
            {
                await TokenHelper.Instance.RefreshTokenAsync(AccountHelper.Instance.GetCurrentInfo().RefreshToken);
                return await GetCommentsAsync(postName, input);
            }
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

            try
            {
                var response = await WebHelper.GetDeserializedResponseAsync<Listing<IList<ApiObjectWithKind<Post>>>>("/.json?" + queryString.ToString());
                return response.Data.Children.Select(p => p.Data).ToList();
            }
            catch (UnauthorizedAccessException)
            {
                await TokenHelper.Instance.RefreshTokenAsync(AccountHelper.Instance.GetCurrentInfo().RefreshToken);
                return await GetFrontpagePostsAsync(sort, listingInput);
            }
        }

        public async Task<User> GetMeAsync()
        {
            try
            {
                return await WebHelper.GetDeserializedResponseAsync<User>("/api/v1/me", true);
            } catch (UnauthorizedAccessException)
            {
                await TokenHelper.Instance.RefreshTokenAsync(AccountHelper.Instance.GetCurrentInfo().RefreshToken);
                return await GetMeAsync();
            }
        }

        public async Task<Subreddit> GetSubredditInfoAsync(string subreddit)
        {
            try
            {
                var response = await WebHelper.GetDeserializedResponseAsync<ApiObjectWithKind<Subreddit>>($"/r/{subreddit}/about.json");

                return response.Data;
            } catch (UnauthorizedAccessException)
            {
                await TokenHelper.Instance.RefreshTokenAsync(AccountHelper.Instance.GetCurrentInfo().RefreshToken);
                return await GetSubredditInfoAsync(subreddit);
            }
        }

        public async Task<IList<Post>> GetSubredditPostsAsync(string subreddit, SortMode sort, ListingInput listingInput)
        {
            try
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
            } catch (UnauthorizedAccessException e)
            {
                await TokenHelper.Instance.RefreshTokenAsync(AccountHelper.Instance.GetCurrentInfo().RefreshToken);
                return await GetSubredditPostsAsync(subreddit, sort, listingInput);
            }
        }

        public async Task<User> GetUserAsync(string userName)
        {
            try
            {
                var response = await WebHelper.GetDeserializedResponseAsync<ApiObjectWithKind<User>>($"/user/{userName}/about.json");
                return response.Data;
            } catch (UnauthorizedAccessException)
            {
                await TokenHelper.Instance.RefreshTokenAsync(AccountHelper.Instance.GetCurrentInfo().RefreshToken);
                return await GetUserAsync(userName);
            }
        }

        public Task<UserKarmaContainer> GetUserKarmaAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Post>> GetUserPostsAsync(string user, SortMode sort, ListingInput listingInput)
        {
            try
            {
                var queryString = HttpUtility.ParseQueryString(string.Empty);

                if (listingInput != null)
                {
                    queryString.Add("after", listingInput.After);
                    queryString.Add("before", listingInput.Before);
                    queryString.Add("limit", listingInput.Limit.ToString());
                }

                var response = await WebHelper.GetDeserializedResponseAsync<Listing<IList<ApiObjectWithKind<Post>>>>($"/user/{user}/submitted/{sort.ToString().ToLower()}.json?{queryString}");

                return response.Data.Children.Select(p => p.Data).ToList();
            } catch (UnauthorizedAccessException)
            {
                await TokenHelper.Instance.RefreshTokenAsync(AccountHelper.Instance.GetCurrentInfo().RefreshToken);
                return await GetUserPostsAsync(user, sort, listingInput);
            }
        }

        public async Task VoteAsync(VotingInput input)
        {
            try
            {
                var queryString = HttpUtility.ParseQueryString(string.Empty);

                if (input != null)
                {
                    queryString.Add("direction", input.Direction.ToString());
                    queryString.Add("id", input.Id);
                }

                await WebHelper.PostAsync("/api/vote", new Dictionary<string, string>());
            } catch (UnauthorizedAccessException)
            {
                await TokenHelper.Instance.RefreshTokenAsync(AccountHelper.Instance.GetCurrentInfo().RefreshToken);
                await VoteAsync(input);
            }
        }
    }
}
