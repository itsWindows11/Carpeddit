using System.Text.Json.Serialization;

namespace Carpeddit.Models
{
    public record LinkFlairRichText(
        [property: JsonPropertyName("e")] string Type,
        [property: JsonPropertyName("t")] string Text
    );
}
