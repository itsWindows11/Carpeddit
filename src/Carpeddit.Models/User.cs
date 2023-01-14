using System.Text.Json.Serialization;

namespace Carpeddit.Models
{
    public sealed class User
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("over_18")]
        public bool Over18 { get; set; }

        [JsonPropertyName("is_friend")]
        public bool IsFriend { get; set; }

        [JsonPropertyName("is_mod")]
        public bool IsMod { get; set; }

        [JsonPropertyName("link_karma")]
        public int LinkKarma { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("modhash")]
        public string ModHash { get; set; }

        [JsonPropertyName("inbox_count")]
        public int? InboxCount { get; set; }

        [JsonPropertyName("comment_karma")]
        public int CommentKarma { get; set; }

        [JsonPropertyName("has_mail")]
        public bool? HasMail { get; set; }

        [JsonPropertyName("has_mod_mail")]
        public bool? HasModMail { get; set; }

        [JsonPropertyName("has_verified_email")]
        public bool HasVerifiedEmail { get; set; }
    }
}
