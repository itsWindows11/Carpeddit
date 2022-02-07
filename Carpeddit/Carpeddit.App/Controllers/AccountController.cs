using Carpeddit.App.Models;
using Carpeddit.App.Other;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace Carpeddit.App.Controllers
{
    public class AccountController
    {
        public static string AccessToken;
        public static string DeviceID;

#if DEBUG
        public static string CLIENT_ID = "mR-hqfet7BP__S3i1ZEplA";
#endif

        public static string CreateDeviceID()
        {
            string deviceId = Guid.NewGuid().ToString();
            App.CurrentAccount.DeviceId = deviceId;
            _ = App.AccDBController.UpdateAsync(App.CurrentAccount);
            return deviceId;
        }

        public static async Task<bool> TryLaunchLoginFlowAsync()
        {
            return await Launcher.LaunchUriAsync(new Uri("https://www.reddit.com/api/v1/authorize?client_id=" + CLIENT_ID + "&response_type=code&state=login&redirect_uri=" + Constants.RedirectUri + "&duration=permanent&scope=creddits modcontributors modmail modconfig subscribe structuredstyles vote wikiedit mysubreddits submit modlog modposts modflair save modothers adsconversions read privatemessages report identity livemanage account modtraffic wikiread edit modwiki modself history flair"));
        }

        public static async Task<AuthViewModel> TryGetTokenInfoAsync(string code)
        {
            try
            {
                HttpRequestMessage msg = new(HttpMethod.Post, "https://www.reddit.com/api/v1/access_token")
                {
                    Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("code", code),
                        new KeyValuePair<string, string>("grant_type", "authorization_code"),
                        new KeyValuePair<string, string>("redirect_uri", Constants.RedirectUri)
                    })
                };

                msg.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(Constants.ClientId + ":" + Constants.ClientSecret)));

                /* 
                 
                  Response looks like this (if successful):

                  {
                        "access_token": Your access token,
                        "token_type": "bearer",
                        "expires_in": Unix Epoch Seconds,
                        "scope": A scope string,
                        "refresh_token": Your refresh token
                  }

                  How to access these?

                  responseObj.access_token  
                  responseObj.token_type
                  responseObj.expires_in
                  responseObj.scope
                  responseObj.refresh_token

                */

                return await GetResultAsync<AuthViewModel>(msg);
            } catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }

        public async Task<dynamic> TryRefreshTokenInfo(string refreshToken)
        {
            try
            {
                HttpResponseMessage msg = await App.GlobalClient.PostAsync("https://www.reddit.com/api/v1/access_token", new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", refreshToken)
                }));

                /* 
                 
                  Response looks like this (if successful):

                  {
                        "access_token": Your access token,
                        "token_type": "bearer",
                        "expires_in": Unix Epoch Seconds,
                        "scope": A scope string
                  }

                  How to access these?

                  responseObj.access_token  
                  responseObj.token_type
                  responseObj.expires_in
                  responseObj.scope

                */

                dynamic responseObj = JsonConvert.DeserializeObject(msg.Content.ToString());
                return responseObj;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }

        /// <summary>
        /// Private method for retrieving result and converting to .NET Type
        /// </summary>
        /// <typeparam name="Response">TResponse</typeparam>
        /// <param name="msg">HttpRequestMessage</param>
        /// <returns></returns>
        private static async Task<Response> GetResultAsync<Response>(HttpRequestMessage msg)
        {
            using var response = await App.GlobalClient.SendAsync(msg);
            using var content = response.Content;
            var responseContent = await content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception(responseContent);

            if (typeof(IConvertible).IsAssignableFrom(typeof(Response)))
                return (Response)Convert.ChangeType(responseContent, typeof(Response));
            return JToken.Parse(responseContent).ToObject<Response>();
        }
    }
}
