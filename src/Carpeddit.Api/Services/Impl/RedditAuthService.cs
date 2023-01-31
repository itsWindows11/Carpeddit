﻿using Carpeddit.Api.Helpers;
using Carpeddit.Api.Models;
using Carpeddit.App.Api.Helpers;
using Carpeddit.Common.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
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
                    return data ??= JsonSerializer.Deserialize<TokenInfo>((string)value);

                return null;
            }
            set
            {
                data = value;
                ApplicationData.Current.LocalSettings.Values["TokenInfo"] = JsonSerializer.Serialize(data);
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

            var tokenInfo = JsonSerializer.Deserialize<TokenInfo>(await content.ReadAsStringAsync());
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

            var tokenInfo = await WebHelper.PostDeserializedResponseAsync<TokenInfo>("/api/v1/access_token", dictionary);

            tokenInfo.RefreshToken = refreshToken;
            ApplicationData.Current.LocalSettings.Values["TokenInfo"] = JsonSerializer.Serialize(tokenInfo);

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

            return WebHelper.PostAsync("/api/v1/access_token", body);
        }
    }
}
