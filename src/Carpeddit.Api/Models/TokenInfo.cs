using System;
using System.Text.Json.Serialization;

namespace Carpeddit.Api.Models
{
    public sealed class TokenInfo
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int? ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        public DateTimeOffset ExpirationTime { get; set; }
    }
}
