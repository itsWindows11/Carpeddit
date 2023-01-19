using Refit;
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
        [Post("/api/v1/access_token")]
        Task<TokenInfo> GetAccessAsync([Body(BodySerializationMethod.UrlEncoded)] IDictionary<string, string> content, [Authorize("Basic")] string token);

        /// <summary>
        /// Revokes the access token provided.
        /// </summary>
        /// <param name="content">The response body.</param>
        /// <param name="authorization">The auth header to use.</param>
        [Post("/api/v1/access_token")]
        Task RevokeAsync([Body(BodySerializationMethod.UrlEncoded)] IDictionary<string, string> content);

        /// <summary>
        /// Gets the access info.
        /// </summary>
        /// <param name="content">The response body.</param>
        /// <param name="authorization">The auth header to use.</param>
        [Post("/api/v1/access_token")]
        Task<TokenInfo> RefreshTokenAsync([Body(BodySerializationMethod.UrlEncoded)] IDictionary<string, string> content, string refreshToken);
    }
}
