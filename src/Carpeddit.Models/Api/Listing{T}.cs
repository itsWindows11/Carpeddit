using System.Text.Json.Serialization;

namespace Carpeddit.Models
{
    public sealed class Listing<T>
    {
        [JsonPropertyName("kind")]
        public string Kind { get; init; }

        [JsonPropertyName("data")]
        public T Data { get; init; }
    }
}
