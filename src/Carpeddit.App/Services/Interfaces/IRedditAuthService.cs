using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Carpeddit.Models;

namespace Carpeddit.App.Services
{
    public interface IRedditAuthService
    {
        /// <summary>
        /// Gets the access info.
        /// </summary>
        /// <param name="content">The response body.</param>
        /// <param name="authorization">The auth header to use.</param>
        [Post("/api/v1/access_token")]
        Task<AccountModel> GetAccessAsync([Body(BodySerializationMethod.UrlEncoded)] IDictionary<string, string> content, [Authorize("Basic")] string token);
    }
}
