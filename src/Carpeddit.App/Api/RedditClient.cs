using Carpeddit.Api.Enums;
using Carpeddit.Api.Helpers;
using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using Carpeddit.App.Api.Models;
using Carpeddit.Common.Extensions;
using Carpeddit.Models;
using Carpeddit.Models.Api;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Carpeddit.Api
{
    public sealed partial class RedditClient
    {
        private TokenInfo _info;
        private Account _account;
        private IRedditAuthService _redditAuthService;
        private IRedditService _redditService;

        public TokenInfo Info => _info;

        public Account Account
        {
            get => _account;
            private set => _account = value;
        }

        public RedditClient(TokenInfo info)
        {
            _info = info;
            _account = string.IsNullOrEmpty(info.AccessToken) ? null : new(info);
            _redditAuthService = App.App.Services.GetService<IRedditAuthService>();
            _redditService = App.App.Services.GetService<IRedditService>();
        }

        public async Task<IEnumerable<Post>> GetFrontPageAsync(ListingInput input = null, SortMode sort = SortMode.Hot)
        {
            await TokenHelper.VerifyTokenValidationAsync(_info);

            var response = await _redditService.GetFrontpagePostsAsync(sort, _info.AccessToken, input);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await TokenHelper.RefreshTokenAsync(_info.RefreshToken);
                response = await _redditService.GetFrontpagePostsAsync(sort, _info.AccessToken, input);
            }

            return response.Content.Data.Children.Select(p => p.Data);
        }

        public async Task<Subreddit> GetSubredditAsync(string subredditName)
        {
            await TokenHelper.VerifyTokenValidationAsync(_info);

            var response = await _redditService.GetSubredditInfoAsync(subredditName, _info.AccessToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await TokenHelper.RefreshTokenAsync(_info.RefreshToken);
                response = await _redditService.GetSubredditInfoAsync(subredditName, _info.AccessToken);
            }

            return response.Content.Data;
        }

        public async Task<IEnumerable<Post>> GetSubredditPostsAsync(string subredditName, ListingInput input = null, SortMode sort = SortMode.Hot)
        {
            await TokenHelper.VerifyTokenValidationAsync(_info);

            var response = await _redditService.GetSubredditPostsAsync(subredditName, sort, _info.AccessToken, input);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await TokenHelper.RefreshTokenAsync(_info.RefreshToken);
                response = await _redditService.GetSubredditPostsAsync(subredditName, sort, _info.AccessToken, input);
            }

            return response.Content.Data.Children.Select(p => p.Data);
        }

        public async Task<IEnumerable<Post>> GetUserPostsAsync(string userName, ListingInput input = null, SortMode sort = SortMode.Hot)
        {
            await TokenHelper.VerifyTokenValidationAsync(_info);

            var response = await _redditService.GetUserPostsAsync(userName, sort, _info.AccessToken, input);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await TokenHelper.RefreshTokenAsync(_info.RefreshToken);
                response = await _redditService.GetUserPostsAsync(userName, sort, _info.AccessToken, input);
            }

            return response.Content.Data.Children.Select(p => p.Data);
        }
    }
}
