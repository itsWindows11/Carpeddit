using System.Text.Json.Serialization;

namespace Carpeddit.Api.Models
{
    public sealed class UserKarma
    {
        [JsonPropertyName("comment_karma")]
        public int CommentKarma { get; set; }

        [JsonPropertyName("link_karma")]
        public int LinkKarma { get; set; }

        [JsonPropertyName("sr")]
        public string Sr { get; set; }
    }
}
