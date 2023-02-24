using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using Carpeddit.Common.Constants;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Carpeddit.Api.Helpers
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
        public void SaveAccessInfo(TokenInfo info, string userName = null)
            => (authService as RedditAuthService).Data = info;

        /// <summary>
        /// Saves the access info.
        /// </summary>
        /// <param name="info">The token info.</param>
        /// <param name="userName">The user name.</param>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        /*public void SaveAccessInfo(PasswordToken info, string userName)
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
        }*/

        /// <summary>
        /// Cleans up locally stored user data, then revokes the access token.
        /// </summary>
        /// <returns>A <see cref="Task" /> which represents the operation.</returns>
        public Task SignOutAsync(bool revokeToken = true)
        {
            var accessToken = GetCurrentInfo()?.AccessToken;

            (authService as RedditAuthService).Data = null;
            (service as RedditService).Me = null;

            if (!revokeToken)
                return Task.CompletedTask;

            return accessToken == null ? Task.CompletedTask : authService.RevokeAsync(accessToken);
        }

        public TokenInfo GetCurrentInfo()
            => (authService as RedditAuthService).Data;
    }
}
