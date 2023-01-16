using System.Text.Json.Serialization;

namespace Carpeddit.Models
{
    public sealed class Listing<T>
    {
        [JsonPropertyName("kind")]
        public string Kind { get; init; }

        [JsonPropertyName("data")]
        public ListingData<T> Data { get; init; }
    }

    public sealed class ListingData<T>
    {
        [JsonPropertyName("after")]
        public string After { get; init; }

        [JsonPropertyName("dist")]
        public int Dist { get; init; }

        [JsonPropertyName("before")]
        public string Before { get; init; }

        [JsonPropertyName("children")]
        public T Children { get; init; }
    }
}
