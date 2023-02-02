using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Carpeddit.Api.Models
{
    public record Image(
        [property: JsonPropertyName("source")] PostImageSource Source,
        [property: JsonPropertyName("resolutions")] IReadOnlyList<Resolution> Resolutions,
        [property: JsonPropertyName("id")] string Id
    );

    public record PostImageSource(
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("width")] int? Width,
        [property: JsonPropertyName("height")] int? Height
    );

    public record Resolution(
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("width")] int? Width,
        [property: JsonPropertyName("height")] int? Height
    );

    public record Preview(
        [property: JsonPropertyName("images")] IReadOnlyList<Image> Images,
        [property: JsonPropertyName("enabled")] bool? Enabled
    );
}
