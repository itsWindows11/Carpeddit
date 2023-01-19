using Carpeddit.Api.Helpers;
using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using Carpeddit.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public async Task<IEnumerable<Post>> GetFrontPageAsync()
        {
            await TokenHelper.VerifyTokenValidationAsync(_info);

            var response = await _redditService.GetFrontpagePostsAsync(_info.AccessToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await TokenHelper.RefreshTokenAsync(_info.RefreshToken);
                response = await _redditService.GetFrontpagePostsAsync(_info.AccessToken);
            }

            return response.Content.Data.Children.Select(p => p.Data);
        }
    }
}
