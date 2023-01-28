using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using Carpeddit.Common.Constants;
using Carpeddit.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace Carpeddit.App.Api.Helpers
{
    public static partial class AccountHelper
    {
        /// <summary>
        /// Gets the access info using the provided code from 
        /// Reddit's authorization page.
        /// </summary>
        /// <param name="code">The provided code from Reddit's authorization page.</param>
        /// <returns>The token info.</returns>
        public static Task<TokenInfo> GetAccessInfoAsync(string code)
        {
            var service = App.Services.GetService<IRedditAuthService>();

            var dictionary = new Dictionary<string, string>()
            {
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", APIConstants.RedirectUri }
            };

            return service.GetAccessAsync(dictionary, Convert.ToBase64String(Encoding.ASCII.GetBytes(APIConstants.ClientId + ":" + APIConstants.ClientSecret)));
        }

        /// <summary>
        /// Saves the access info.
        /// </summary>
        /// <param name="info">The token info.</param>
        /// <param name="userName">The user name, if not specified then it gets automatically retrieved from the API.</param>
        /// <returns>A <see cref="Task"/> which represents the operation.</returns>
        public static async Task SaveAccessInfoAsync(PasswordToken info, string userName = null)
        {
            var passwordValut = new PasswordVault();
            var credentials = passwordValut.RetrieveAll();

            // Fetch user info.
            if (string.IsNullOrEmpty(userName))
                userName = (await App.Client.Account.GetMeUnsafeAsync()).Name;

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
        public static Task SignOutAsync(bool revokeToken = true)
        {
            foreach (var credential in App.Valut.RetrieveAll())
                App.Valut.Remove(credential);

            if (!revokeToken)
                return Task.CompletedTask;

            // Even though token_type_hint is optional, the response
            // should be completed as quick as possible.
            var body = new Dictionary<string, string>()
            {
                { "token", App.Client.Info.AccessToken },
                { "token_type_hint", "access_token" }
            };

            return App.Services.GetService<IRedditAuthService>().RevokeAsync(body);
        }

        public static AccountModel GetCurrentInfo()
        {
            var passwordValut = new PasswordVault();
            var credential = passwordValut.RetrieveAll().FirstOrDefault();

            AccountModel result = null;

            if (credential != null)
            {
                credential.RetrievePassword();

                var tokens = credential.Password.Split(':', StringSplitOptions.RemoveEmptyEntries);

                result = new()
                {
                    UserName = credential.UserName,
                    AccessToken = tokens[0],
                    RefreshToken = tokens[1]
                };
            }

            return result;
        }
    }
}
