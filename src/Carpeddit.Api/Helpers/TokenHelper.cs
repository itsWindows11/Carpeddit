using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Carpeddit.Api.Helpers
{
    public sealed class TokenHelper
    {
        public static async Task<TokenInfo> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new ArgumentException($"{nameof(refreshToken)} cannot be null or empty.");

            var authInfo = await Ioc.Default.GetService<IRedditAuthService>().RefreshTokenAsync(refreshToken);

            authInfo.RefreshToken = refreshToken;

            AccountHelper.SaveAccessInfo(authInfo);

            return authInfo;
        }
    }
}
