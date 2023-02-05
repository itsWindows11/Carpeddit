using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Carpeddit.Api.Models
{
    public sealed class ImageContainer
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        // The only value I know of here is "Image".
        [JsonPropertyName("e")]
        public string E { get; set; }

        [JsonPropertyName("m")]
        public string Metadata { get; set; }

        [JsonPropertyName("p")]
        public IReadOnlyList<Image> ImageVersions { get; set; }

        [JsonPropertyName("s")]
        public Image OriginalImage { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
