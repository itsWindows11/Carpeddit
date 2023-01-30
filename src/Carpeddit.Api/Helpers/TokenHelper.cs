using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using Carpeddit.App.Api.Helpers;
using System;
using System.Threading.Tasks;

namespace Carpeddit.Api.Helpers
{
    public sealed class TokenHelper
    {
        public static TokenHelper Instance { get; private set; }

        private IRedditAuthService service;

        public TokenHelper(IRedditAuthService service)
        {
            Instance = this;
            this.service = service;
        }

        public async Task<TokenInfo> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new ArgumentException($"{nameof(refreshToken)} cannot be null or empty.");

            var authInfo = await service.RefreshTokenAsync(refreshToken);

            authInfo.RefreshToken = refreshToken;

            await AccountHelper.Instance.SaveAccessInfoAsync(new()
            {
                AccessToken = authInfo.AccessToken,
                RefreshToken = authInfo.RefreshToken
            });

            return authInfo;
        }
    }
}
