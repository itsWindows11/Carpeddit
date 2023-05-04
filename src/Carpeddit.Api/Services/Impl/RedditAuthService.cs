using Carpeddit.Api.Helpers;
using Carpeddit.Api.Models;
using Carpeddit.Common.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace Carpeddit.Api.Services
{
    public sealed class RedditAuthService : IRedditAuthService
    {
        private TokenInfo data;

        public TokenInfo Data
        {
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.TryGetValue("TokenInfo", out object value))
                    return data ??= JsonSerializer.Deserialize((string)value, ApiJsonContext.Default.TokenInfo);

                return null;
            }
            set
            {
                data = value;
                ApplicationData.Current.LocalSettings.Values["TokenInfo"] = JsonSerializer.Serialize(data, ApiJsonContext.Default.TokenInfo);
            }
        }

        public async Task<TokenInfo> GetAccessAsync(string code, string token)
        {
            var dictionary = new Dictionary<string, string>()
            {
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", APIConstants.RedirectUri }
            };

            var content = await WebHelper.MakePostRequestAsync("https://www.reddit.com/api/v1/access_token", new Dictionary<string, string>()
            {
                { "Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(APIConstants.ClientId + ":" + APIConstants.ClientSecret)) }
            }, dictionary);

            var tokenInfo = JsonSerializer.Deserialize(await content.ReadAsStringAsync(), ApiJsonContext.Default.TokenInfo);
            tokenInfo.ExpirationTime = DateTimeOffset.Now.AddSeconds((double)tokenInfo.ExpiresIn);
            Data = tokenInfo;

            return tokenInfo;
        }

        public async Task<TokenInfo> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new ArgumentException($"{nameof(refreshToken)} cannot be null or empty.");

            var dictionary = new Dictionary<string, string>()
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };

            var content = await WebHelper.MakePostRequestAsync("https://www.reddit.com/api/v1/access_token", new Dictionary<string, string>()
            {
                { "Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(APIConstants.ClientId + ":" + APIConstants.ClientSecret)) }
            }, dictionary);

            var tokenInfo = JsonSerializer.Deserialize(await content.ReadAsStringAsync(), ApiJsonContext.Default.TokenInfo);

            tokenInfo.RefreshToken = refreshToken;
            ApplicationData.Current.LocalSettings.Values["TokenInfo"] = JsonSerializer.Serialize(tokenInfo, ApiJsonContext.Default.TokenInfo);

            return tokenInfo;
        }

        public Task RevokeAsync(string accessToken)
        {
            // Even though token_type_hint is optional, the response
            // should be completed as quick as possible.
            var body = new Dictionary<string, string>()
            {
                { "token", accessToken },
                { "token_type_hint", "access_token" }
            };

            return WebHelper.MakePostRequestAsync("https://www.reddit.com/api/v1/revoke_token", new Dictionary<string, string>()
            {
                { "Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(APIConstants.ClientId + ":" + APIConstants.ClientSecret)) }
            }, body);
        }
    }
}
