using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using Carpeddit.Common.Constants;
using Carpeddit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage;

namespace Carpeddit.App.Api.Helpers
{
    public sealed partial class AccountHelper
    {
        private IRedditAuthService authService;
        private IRedditService service;

        public static AccountHelper Instance { get; private set; }

        public AccountHelper(IRedditAuthService authService, IRedditService service)
        {
            this.authService = authService;
            this.service = service;
            Instance = this;
        }

        /// <summary>
        /// Gets the access info using the provided code from 
        /// Reddit's authorization page.
        /// </summary>
        /// <param name="code">The provided code from Reddit's authorization page.</param>
        /// <returns>The token info.</returns>
        public Task<TokenInfo> GetAccessInfoAsync(string code)
            => authService.GetAccessAsync(code, Convert.ToBase64String(Encoding.ASCII.GetBytes(APIConstants.ClientId + ":" + APIConstants.ClientSecret)));

        /// <summary>
        /// Saves the access info.
        /// </summary>
        /// <param name="info">The token info.</param>
        /// <param name="userName">The user name, if not specified then it gets automatically retrieved from the API.</param>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        public async Task SaveAccessInfoAsync(PasswordToken info, string userName = null)
        {
            var passwordValut = new PasswordVault();
            var credentials = passwordValut.RetrieveAll();

            // Fetch user info.
            if (string.IsNullOrEmpty(userName))
                userName = (await service.GetMeAsync()).Name;

            // Remove any existing entry.
            // Should only allow for one account to be
            // authenticated at once. But makes it easier
            // to handle auth in the long run.
            if (credentials.Any())
            {
                foreach (var credential in credentials)
                    passwordValut.Remove(credential);
            }

            passwordValut.Add(new("Reddit", userName, $"{info.AccessToken}:{info.RefreshToken}"));
        }

        /// <summary>
        /// Saves the access info.
        /// </summary>
        /// <param name="info">The token info.</param>
        /// <param name="userName">The user name.</param>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        public void SaveAccessInfo(PasswordToken info, string userName)
        {
            var passwordValut = new PasswordVault();
            var credentials = passwordValut.RetrieveAll();

            // Remove any existing entry.
            // Should only allow for one account to be
            // authenticated at once. But makes it easier
            // to handle auth in the long run.
            if (credentials.Any())
            {
                foreach (var credential in credentials)
                    passwordValut.Remove(credential);
            }

            passwordValut.Add(new("Reddit", userName, $"{info.AccessToken}:{info.RefreshToken}"));
        }

        /// <summary>
        /// Cleans up locally stored tokens, then revokes the access token.
        /// </summary>
        /// <returns>A <see cref="Task" /> which represents the operation.</returns>
        public async Task SignOutAsync(bool revokeToken = true)
        {
            (authService as RedditAuthService).Data = null;

            if (!revokeToken)
                return;

            await authService.RevokeAsync(GetCurrentInfo().AccessToken);
        }

        public TokenInfo GetCurrentInfo()
            => (authService as RedditAuthService).Data;
    }
}
