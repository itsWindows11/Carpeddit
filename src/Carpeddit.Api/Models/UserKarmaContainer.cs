using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Carpeddit.Api.Models
{
    public sealed class UserKarmaContainer
    {
        [JsonPropertyName("data")]
        public IEnumerable<UserKarma> Data { get; set; }
    }
}
