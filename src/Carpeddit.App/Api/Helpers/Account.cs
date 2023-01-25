using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using Carpeddit.Models;
using Carpeddit.Models.Api;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Carpeddit.Api.Helpers
{
    public sealed class Account
    {
        private User _me;
        private TokenInfo _info;

        internal Account(TokenInfo info)
            => _info = info;

        public async Task<User> GetMeUnsafeAsync()
        {
            var service = App.App.Services.GetService<IRedditService>();

            var response = await service.GetCurrentlyAuthenticatedUserAsync(_info.AccessToken);

            return response.Content;
        }

        public async Task<User> GetMeAsync()
        {
            if (_me != null)
                return _me;

            await TokenHelper.VerifyTokenValidationAsync(_info);

            var service = App.App.Services.GetService<IRedditService>();

            var response = await service.GetCurrentlyAuthenticatedUserAsync(_info.AccessToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await TokenHelper.RefreshTokenAsync(_info.RefreshToken);
                response = await service.GetCurrentlyAuthenticatedUserAsync(_info.AccessToken);
            }

            return _me = response.Content;
        }

        public async Task<IEnumerable<UserKarma>> GetKarmaAsync()
        {
            await TokenHelper.VerifyTokenValidationAsync(_info);
            return (await App.App.Services.GetService<IRedditService>().GetUserKarmaAsync(_info.AccessToken)).Content.Data;
        }
    }
}
