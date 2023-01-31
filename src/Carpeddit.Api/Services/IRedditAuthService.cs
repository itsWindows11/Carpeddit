using System.Collections.Generic;
using System.Threading.Tasks;
using Carpeddit.Api.Models;

namespace Carpeddit.Api.Services
{
    public interface IRedditAuthService
    {
        /// <summary>
        /// Gets the access info.
        /// </summary>
        /// <param name="content">The response body.</param>
        /// <param name="authorization">The auth header to use.</param>
        Task<TokenInfo> GetAccessAsync(string code, string token);

        /// <summary>
        /// Revokes the access token provided.
        /// </summary>
        /// <param name="content">The response body.</param>
        /// <param name="authorization">The auth header to use.</param>
        Task RevokeAsync(string accessToken);

        /// <summary>
        /// Refreshes the access token with the provided refresh token.
        /// </summary>
        /// <param name="content">The response body.</param>
        /// <param name="authorization">The auth header to use.</param>
        Task<TokenInfo> RefreshTokenAsync(string refreshToken);
    }
}
