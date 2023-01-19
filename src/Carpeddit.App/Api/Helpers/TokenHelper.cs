using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carpeddit.Api.Helpers
{
    public static class TokenHelper
    {
        public static async Task<TokenInfo> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new ArgumentException($"{nameof(refreshToken)} cannot be null or empty.");

            var dictionary = new Dictionary<string, string>()
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };

            var authInfo = await App.App.Services.GetService<IRedditAuthService>().RefreshTokenAsync(dictionary, refreshToken);

            authInfo.RefreshToken = refreshToken;

            return authInfo;
        }

        public static async Task VerifyTokenValidationAsync(TokenInfo info)
        {
            if (string.IsNullOrEmpty(info.AccessToken) || (info.UtcExpirationTime != DateTimeOffset.MinValue && DateTime.UtcNow >= info.UtcExpirationTime))
                info = await RefreshTokenAsync(info.RefreshToken);
        }
    }
}
