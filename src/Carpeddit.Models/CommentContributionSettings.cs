using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Carpeddit.Models.Api
{
    public sealed class CommentContributionSettings
    {
        [JsonPropertyName("allowed_media_types")]
        public JsonArray? AllowedMediaTypes { get; set; }
    }
}
