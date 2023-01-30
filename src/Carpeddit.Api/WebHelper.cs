using Carpeddit.Api.Exceptions;
using Carpeddit.App.Api.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace Carpeddit.Api
{
    // Variables and helpers
    public static partial class WebHelper
    {
        private static HttpClient httpClient = new();

        /// <summary>
        /// Directly gets a string from the remote server.
        /// </summary>
        /// <param name="url">The URL of the server.</param>
        /// <returns>The response, as a string.</returns>
        public static async Task<string> GetStringAsync(string url, bool oauthOnly = false)
        {
            var content = await GetAsync(url, oauthOnly);
            return await content.ReadAsStringAsync();
        }

        /// <summary>
        /// Directly gets a string from the remote server.
        /// </summary>
        /// <param name="url">The URL of the server.</param>
        /// <returns>The response, as a string.</returns>
        public static async Task<string> PostStringAsync(string url, IDictionary<string, string> postData, bool oauthOnly = false)
        {
            var content = await PostAsync(url, postData, oauthOnly);
            return await content.ReadAsStringAsync();
        }

        /// <summary>
        /// Directly gets a string from the remote server.
        /// </summary>
        /// <param name="url">The URL of the server.</param>
        /// <returns>The response, as a string.</returns>
        public static async Task<string> PatchStringAsync(string url, IDictionary<string, string> patchData, bool oauthOnly = false)
        {
            var content = await PatchAsync(url, patchData, oauthOnly);
            return await content.ReadAsStringAsync();
        }

        /// <summary>
        /// Gets a response from the remote server, then deserializes it automatically.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="url">The URL of the server.</param>
        /// <returns>The deserialized response.</returns>
        public static async Task<T> GetDeserializedResponseAsync<T>(string url, bool oauthOnly = false)
        {
            var content = await GetStringAsync(url, oauthOnly);
            return JsonSerializer.Deserialize<T>(content);
        }

        /// <summary>
        /// Gets a response from the remote server, then deserializes it automatically.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="url">The URL of the server.</param>
        /// <returns>The deserialized response.</returns>
        public static async Task<T> PostDeserializedResponseAsync<T>(string url, IDictionary<string, string> postData, bool oauthOnly = false)
        {
            var content = await PostAsync(url, postData, oauthOnly);
            using var stream = await content.ReadAsInputStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(stream.AsStreamForRead());
        }

        /// <summary>
        /// Gets a response from the remote server, then deserializes it automatically.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="url">The URL of the server.</param>
        /// <returns>The deserialized response.</returns>
        public static async Task<T> PatchDeserializedResponseAsync<T>(string url, IDictionary<string, string> patchData, bool oauthOnly = false)
        {
            var content = await PatchAsync(url, patchData, oauthOnly);
            using var stream = await content.ReadAsInputStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(stream.AsStreamForRead());
        }
    }

    // More helper methods.
    public static partial class WebHelper
    {
        public static async Task<IHttpContent> GetAsync(string url, bool oauthOnly = false)
        {
            var info = await AccountHelper.Instance.GetCurrentInfo();

            if (info != null)
            {
                var neededHeaders = new Dictionary<string, string>()
                {
                    { "Authorization", $"bearer {info.AccessToken}" }
                };

                return await MakeGetRequestAsync("https://oauth.reddit.com" + url, neededHeaders);
            }

            return oauthOnly ? new HttpStringContent(string.Empty) : await MakeGetRequestAsync("https://www.reddit.com" + url, new Dictionary<string, string>());
        }

        public static async Task<IHttpContent> PostAsync(string url, IDictionary<string, string> postData, bool oauthOnly = false)
        {
            var info = await AccountHelper.Instance.GetCurrentInfo();

            if (info != null)
            {
                var neededHeaders = new Dictionary<string, string>()
                {
                    { "Authorization", $"bearer {info.AccessToken}" }
                };

                return await MakePostRequestAsync("https://oauth.reddit.com" + url, neededHeaders, postData);
            }

            return oauthOnly ? new HttpStringContent(string.Empty) : await MakePostRequestAsync("https://www.reddit.com" + url, new Dictionary<string, string>(), postData);
        }

        public static async Task<IHttpContent> PatchAsync(string url, IDictionary<string, string> postData, bool oauthOnly = false)
        {
            var info = await AccountHelper.Instance.GetCurrentInfo();

            if (info != null)
            {
                var neededHeaders = new Dictionary<string, string>()
                {
                    { "Authorization", $"bearer {info.AccessToken}" }
                };

                return await MakePatchRequestAsync("https://oauth.reddit.com" + url, neededHeaders, postData);
            }

            return oauthOnly ? new HttpStringContent(string.Empty) : await MakePatchRequestAsync("https://www.reddit.com" + url, new Dictionary<string, string>(), postData);
        }
    }
    
    // Generic methods for web requests.
    public static partial class WebHelper
    {
        /// <summary>
        /// Makes a generic GET request.
        /// </summary>
        /// <param name="url">The URL to make a request to.</param>
        /// <returns>The HTTP content.</returns>
        public static async Task<IHttpContent> MakeGetRequestAsync(string url, IDictionary<string, string> headers)
        {
            var response = await httpClient.SendRequestAsync(MakeMessage(url, HttpMethod.Get, headers),
                HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                response.StatusCode == HttpStatusCode.BadGateway ||
                response.StatusCode == HttpStatusCode.GatewayTimeout ||
                response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new ServiceDownException();
            }

            return response.Content;
        }

        /// <summary>
        /// Makes a generic POST request.
        /// </summary>
        /// <param name="url">The URL to make a request to.</param>
        /// <returns>The HTTP content.</returns>
        public static async Task<IHttpContent> MakePostRequestAsync(string url, IDictionary<string, string> headers, IDictionary<string, string> postData)
        {
            var message = MakeMessage(url, HttpMethod.Post, headers);
            message.Content = new HttpFormUrlEncodedContent(postData);

            var response = await httpClient.SendRequestAsync(message, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                response.StatusCode == HttpStatusCode.BadGateway ||
                response.StatusCode == HttpStatusCode.GatewayTimeout ||
                response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new ServiceDownException();
            }

            return response.Content;
        }

        /// <summary>
        /// Makes a generic PATCH request.
        /// </summary>
        /// <param name="url">The URL to make a request to.</param>
        /// <returns>The HTTP content.</returns>
        public static async Task<IHttpContent> MakePatchRequestAsync(string url, IDictionary<string, string> headers, IDictionary<string, string> patchData)
        {
            var message = MakeMessage(url, HttpMethod.Patch, headers);
            message.Content = new HttpFormUrlEncodedContent(patchData);

            var response = await httpClient.SendRequestAsync(message, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                response.StatusCode == HttpStatusCode.BadGateway ||
                response.StatusCode == HttpStatusCode.GatewayTimeout ||
                response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new ServiceDownException();
            }

            return response.Content;
        }

        private static HttpRequestMessage MakeMessage(string url, HttpMethod method, IDictionary<string, string> headers)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("The URL cannot be null.");

            var message = new HttpRequestMessage(method, new Uri(url, UriKind.Absolute));

            message.Headers.Add("User-Agent", "Carpeddit");

            foreach (var header in headers)
                message.Headers.Add(header);

            return message;
        }
    }
}
