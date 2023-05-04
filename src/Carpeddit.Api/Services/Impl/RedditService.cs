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
        public User Me { get; set; }

        public Task<IList<Comment>> GetCommentsAsync(string postName, ListingInput input)
            => RunAsync<IList<Comment>>(async () =>
            {
                var response = await WebHelper.GetDeserializedResponseAsync($"/comments/{postName}?raw_json=1", ApiJsonContext.Default.IListListingIListApiObjectWithKindComment);

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
                        return JsonSerializer.Deserialize<More>(obj.Data.ToString(), ApiJsonContext.Default.More);

                    return JsonSerializer.Deserialize<Comment>(obj.Data.ToString(), ApiJsonContext.Default.Comment);
                }).ToList();
            });

        public Task<IList<Post>> GetFrontpagePostsAsync(SortMode sort, ListingInput listingInput = null)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            if (listingInput != null)
            {
                queryString.Add("after", listingInput.After);
                queryString.Add("raw_json", "1");
                queryString.Add("before", listingInput.Before);
                queryString.Add("limit", listingInput.Limit.ToString());

                var sortString = sort.ToString();

                if (sortString.EndsWith("All Time"))
                {
                    queryString.Add("t", "all");
                } else if (sortString.EndsWith("Today"))
                {
                    queryString.Add("t", "day");
                }
                else if (sortString.EndsWith("Now"))
                {
                    queryString.Add("t", "hour");
                } else
                {
                    queryString.Add("t", sortString.Replace("Top", string.Empty).Replace("Controversial", string.Empty).ToLower());
                }
            }

            return RunAsync<IList<Post>>(async () =>
            {
                var response = await WebHelper.GetDeserializedResponseAsync($"/{StringToSortTypeConverter.ToAPISort(sort)}.json?{queryString}", ApiJsonContext.Default.ListingIListApiObjectWithKindPost);
                return response.Data.Children.Select(p => p.Data).ToList();
            });
        }

        public async Task<User> GetMeAsync()
            => Me ??= await RunAsync(() => WebHelper.GetDeserializedResponseAsync("/api/v1/me?raw_json=1", ApiJsonContext.Default.User, true));

        public Task<Subreddit> GetSubredditInfoAsync(string subreddit)
            => RunAsync(async () => (await WebHelper.GetDeserializedResponseAsync($"/r/{subreddit}/about.json?raw_json=1", ApiJsonContext.Default.ApiObjectWithKindSubreddit)).Data);

        public Task<IList<Post>> GetSubredditPostsAsync(string subreddit, SortMode sort, ListingInput listingInput)
            => RunAsync<IList<Post>>(async () =>
            {
                var queryString = HttpUtility.ParseQueryString(string.Empty);

                if (listingInput != null)
                {
                    queryString.Add("after", listingInput.After);
                    queryString.Add("raw_json", "1");
                    queryString.Add("before", listingInput.Before);
                    queryString.Add("limit", listingInput.Limit.ToString());

                    var sortString = sort.ToString();

                    if (sortString.EndsWith("All Time"))
                    {
                        queryString.Add("t", "all");
                    }
                    else if (sortString.EndsWith("Today"))
                    {
                        queryString.Add("t", "day");
                    }
                    else if (sortString.EndsWith("Now"))
                    {
                        queryString.Add("t", "hour");
                    }
                    else
                    {
                        queryString.Add("t", sortString.Replace("Top", string.Empty).Replace("Controversial", string.Empty).ToLower());
                    }
                }

                var listing = await WebHelper.GetDeserializedResponseAsync($"/r/{subreddit}/{StringToSortTypeConverter.ToAPISort(sort)}.json?{queryString}", ApiJsonContext.Default.ListingIListApiObjectWithKindPost);

                return listing.Data.Children.Select(p => p.Data).ToList();
            });

        public Task<User> GetUserAsync(string userName)
            => RunAsync(async () => (await WebHelper.GetDeserializedResponseAsync($"/user/{userName}/about.json?raw_json=1", ApiJsonContext.Default.ApiObjectWithKindUser)).Data);

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
                    queryString.Add("raw_json", "1");
                    queryString.Add("before", listingInput.Before);
                    queryString.Add("limit", listingInput.Limit.ToString());

                    var sortString = sort.ToString();

                    if (sortString.EndsWith("All Time"))
                    {
                        queryString.Add("t", "all");
                    }
                    else if (sortString.EndsWith("Today"))
                    {
                        queryString.Add("t", "day");
                    }
                    else if (sortString.EndsWith("Now"))
                    {
                        queryString.Add("t", "hour");
                    }
                    else
                    {
                        queryString.Add("t", sortString.Replace("Top", string.Empty).Replace("Controversial", string.Empty).ToLower());
                    }
                }

                var response = await WebHelper.GetDeserializedResponseAsync($"/user/{user}/submitted/{StringToSortTypeConverter.ToAPISort(sort)}.json?{queryString}", ApiJsonContext.Default.ListingIListApiObjectWithKindPost);

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

        public Task SubscribeToSubredditsAsync(IEnumerable<string> subreddits, bool srName = false)
            => RunAsync(() =>
            {
                return WebHelper.PostAsync($"/api/subscribe", new Dictionary<string, string>()
                {
                    { "action", "sub" },
                    { "action_source", "o" },
                    { "skip_initial_defaults", "false" },
                    { srName ? "sr_name" : "sr", string.Join(", ", subreddits) }
                });
            });

        public Task UnsubscribeFromSubredditsAsync(IEnumerable<string> subreddits, bool srName = false)
            => RunAsync(() =>
            {
                return WebHelper.PostAsync($"/api/subscribe", new Dictionary<string, string>()
                {
                    { "action", "unsub" },
                    { "action_source", "o" },
                    { srName ? "sr_name" : "sr", string.Join(", ", subreddits) }
                });
            });

        public Task<IList<Message>> GetMessagesAsync(MessageListType type = MessageListType.Inbox)
            => RunAsync<IList<Message>>(async () =>
            {
                var messagesListing = await WebHelper.GetDeserializedResponseAsync($"/message/{type.ToString().ToLower()}", ApiJsonContext.Default.ListingIListApiObjectWithKindMessage);

                return messagesListing.Data.Children.Select(a => a.Data).ToList();
            });

        public Task CommentAsync(string fullname, string rawMarkdownText)
            => RunAsync(() =>
            {
                return WebHelper.PostAsync($"/api/comment", new Dictionary<string, string>()
                {
                    { "text", rawMarkdownText },
                    { "thing_id", fullname },
                    { "return_rtjson", "true" }
                });
            });

        public Task SaveAsync(string fullname)
            => RunAsync(() =>
            {
                return WebHelper.PostAsync($"/api/save", new Dictionary<string, string>()
                {
                    { "id", fullname }
                });
            });

        public Task UnsaveAsync(string fullname)
            => RunAsync(() =>
            {
                return WebHelper.PostAsync($"/api/unsave", new Dictionary<string, string>()
                {
                    { "id", fullname }
                });
            });
    }

    // Helper methods
    public partial class RedditService
    {
        public async Task RunAsync(Func<Task> func)
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

                var info = AccountHelper.GetCurrentInfo();

                if (info != null)
                    _ = await TokenHelper.RefreshTokenAsync(info.RefreshToken);

                await func();
            }
        }

        public async Task<T> RunAsync<T>(Func<Task<T>> func)
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

                var info = AccountHelper.GetCurrentInfo();

                if (info != null)
                    _ = await TokenHelper.RefreshTokenAsync(info.RefreshToken);

                return await func();
            }
        }
    }
}
