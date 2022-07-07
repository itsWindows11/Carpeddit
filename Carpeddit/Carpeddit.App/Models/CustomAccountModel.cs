using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpeddit.App.Models
{
    public class CustomAccountModel
    {
        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int? TokenExpiresIn { get; set; }
        
        public bool LoggedIn { get; set; }
        
        public string DeviceId { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}
