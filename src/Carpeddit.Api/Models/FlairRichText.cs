using System.Text.Json.Serialization;

namespace Carpeddit.Api.Models
{
    public record FlairRichText(
        [property: JsonPropertyName("a")] string EmojiName,
        [property: JsonPropertyName("u")] string EmojiUrl,
        [property: JsonPropertyName("e")] string Type,
        [property: JsonPropertyName("t")] string Text
    );
}