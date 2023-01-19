using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using Carpeddit.Models;
using Carpeddit.Models.Api;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carpeddit.Api.Helpers
{
    public sealed class Account
    {
        private User _me;
        private TokenInfo _info;

        internal Account(TokenInfo info)
            => _info = info;

        public async Task<User> GetMeAsync()
        {
            if (_me != null)
                return _me;

            await TokenHelper.VerifyTokenValidationAsync(_info);
            return _me = (await App.App.Services.GetService<IRedditService>().GetCurrentlyAuthenticatedUserAsync(_info.AccessToken)).Content;
        }

        public async Task<IEnumerable<UserKarma>> GetKarmaAsync()
        {
            await TokenHelper.VerifyTokenValidationAsync(_info);
            return (await App.App.Services.GetService<IRedditService>().GetUserKarmaAsync(_info.AccessToken)).Content.Data;
        }
    }
}
