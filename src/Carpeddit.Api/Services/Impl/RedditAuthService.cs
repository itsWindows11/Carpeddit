using Carpeddit.Api.Helpers;
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
        private ManualResetEvent refreshEvent = new(false);
        private bool isTokenRefreshing;
        private bool isTokenRefreshFailed;

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

        public async Task<TokenInfo> GetAccessAsync()
        {
            if (Data == null)
                return null;

            bool shouldRefresh = false;
            bool shouldWait = false;

            lock (refreshEvent)
            {
                TimeSpan timeRemaining = data.ExpirationTime - DateTime.Now;

                // If it is already expired or will do so soon wait on the refresh before using it.
                if (timeRemaining.TotalSeconds < 30 || isTokenRefreshFailed)
                {
                    // Check if someone else is refreshing
                    if (!isTokenRefreshing)
                    {
                        isTokenRefreshing = true;
                        shouldRefresh = true;
                    }
                    shouldWait = true;
                }
                // If it is going to expire soon but not too soon refresh it async.
                else if (timeRemaining.TotalMinutes < 5)
                {
                    // Check if someone else it refreshing
                    if (!isTokenRefreshing)
                    {
                        isTokenRefreshing = true;
                        shouldRefresh = true;
                    }
                }
            }

            // If we should refresh kick off a task to do so.
            if (shouldRefresh)
            {
                new Task(async () =>
                {
                    // Try to refresh
                    try
                    {
                        TokenInfo result = await TokenHelper.Instance.RefreshTokenAsync(data.RefreshToken);
                        isTokenRefreshFailed = result == null;
                    }
                    catch
                    {
                        isTokenRefreshFailed = true;
                    }

                    // Lock the refresh event
                    lock (refreshEvent)
                    {
                        isTokenRefreshing = false;
                        refreshEvent.Set();
                    }
                }).Start();
            }

            if (shouldWait)
            {
                await Task.Run(() =>
                {
                    refreshEvent.WaitOne();
                });
            }

            // Now return the key.
            return isTokenRefreshFailed ? null : data;
        }
    }
}
