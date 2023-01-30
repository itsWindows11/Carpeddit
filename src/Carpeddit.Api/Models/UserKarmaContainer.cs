using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Carpeddit.Models.Api
{
    public sealed class UserKarmaContainer
    {
        [JsonPropertyName("data")]
        public IEnumerable<UserKarma> Data { get; set; }
    }
}
