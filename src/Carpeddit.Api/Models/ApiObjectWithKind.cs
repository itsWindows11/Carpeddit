using System.Text.Json.Serialization;

namespace Carpeddit.Models.Api
{
    public class ApiObjectWithKind<T>
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }
    }
}
