using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using Carpeddit.Common.Constants;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Carpeddit.Api.Helpers
{
    public sealed partial class AccountHelper
    {
        /// <summary>
        /// Gets the access info using the provided code from 
        /// Reddit's authorization page.
        /// </summary>
        /// <param name="code">The provided code from Reddit's authorization page.</param>
        /// <returns>The token info.</returns>
        public static Task<TokenInfo> GetAccessInfoAsync(string code)
            => Ioc.Default.GetService<IRedditAuthService>().GetAccessAsync(code, Convert.ToBase64String(Encoding.ASCII.GetBytes(APIConstants.ClientId + ":" + APIConstants.ClientSecret)));

        /// <summary>
        /// Saves the access info.
        /// </summary>
        /// <param name="info">The token info.</param>
        /// <param name="userName">The user name, if not specified then it gets automatically retrieved from the API.</param>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        public static void SaveAccessInfo(TokenInfo info)
            => (Ioc.Default.GetService<IRedditAuthService>() as RedditAuthService).Data = info;

        /// <summary>
        /// Cleans up locally stored user data, then revokes the access token.
        /// </summary>
        /// <returns>A <see cref="Task" /> which represents the operation.</returns>
        public static Task SignOutAsync(bool revokeToken = true)
        {
            var accessToken = GetCurrentInfo()?.AccessToken;

            (Ioc.Default.GetService<IRedditAuthService>() as RedditAuthService).Data = null;
            (Ioc.Default.GetService<IRedditService>() as RedditService).Me = null;

            if (!revokeToken)
                return Task.CompletedTask;

            return accessToken == null ? Task.CompletedTask : Ioc.Default.GetService<IRedditAuthService>().RevokeAsync(accessToken);
        }

        public static TokenInfo GetCurrentInfo()
            => (Ioc.Default.GetService<IRedditAuthService>() as RedditAuthService).Data;
    }
}
