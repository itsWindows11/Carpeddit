using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Carpeddit.Api.Models
{
    public sealed class Listing<T>
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("data")]
        public ListingData<T> Data { get; set; }
    }

    public sealed class ListingData<T>
    {
        [JsonPropertyName("after")]
        public string After { get; set; }

        [JsonPropertyName("dist")]
        public JsonNode Dist { get; set; }

        [JsonPropertyName("before")]
        public string Before { get; set; }

        [JsonPropertyName("children")]
        public T Children { get; set; }
    }
}
