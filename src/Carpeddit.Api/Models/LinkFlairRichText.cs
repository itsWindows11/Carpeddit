using System.Text.Json.Serialization;

namespace Carpeddit.Api.Models
{
    public record LinkFlairRichText(
        [property: JsonPropertyName("e")] string Type,
        [property: JsonPropertyName("t")] string Text
    );
}
