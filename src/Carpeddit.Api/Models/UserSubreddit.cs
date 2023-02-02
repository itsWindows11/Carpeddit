using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Carpeddit.Api.Models
{
    public sealed class UserSubreddit
    {
        [JsonPropertyName("default_set")]
        public bool DefaultSet { get; set; }

        [JsonPropertyName("user_is_contributor")]
        public bool UserIsContributor { get; set; }

        [JsonPropertyName("banner_img")]
        public string BannerImage { get; set; }

        [JsonPropertyName("restrict_posting")]
        public bool RestrictPosting { get; set; }

        [JsonPropertyName("user_is_banned")]
        public bool UserIsBanned { get; set; }

        [JsonPropertyName("free_form_reports")]
        public bool FreeFormReports { get; set; }

        [JsonPropertyName("community_icon")]
        public string CommunityIcon { get; set; }

        [JsonPropertyName("show_media")]
        public bool ShowMedia { get; set; }

        [JsonPropertyName("icon_color")]
        public string IconColor { get; set; }

        [JsonPropertyName("user_is_muted")]
        public bool? UserIsMuted { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("header_img")]
        public string HeaderImage { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("coins")]
        public int Coins { get; set; }

        [JsonPropertyName("previous_names")]
        public JsonArray PreviousUsernames { get; set; }

        [JsonPropertyName("over_18")]
        public bool Over18 { get; set; }

        [JsonPropertyName("icon_size")]
        public IEnumerable<int> IconSizes { get; set; }

        [JsonPropertyName("primary_color")]
        public string PrimaryColor { get; set; }

        [JsonPropertyName("icon_img")]
        public string IconImage { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("header_size")]
        public int? HeaderSize { get; set; }

        [JsonPropertyName("restrict_commenting")]
        public bool RestrictCommenting { get; set; }

        [JsonPropertyName("subscribers")]
        public int Subscribers { get; set; }

        [JsonPropertyName("submit_text_label")]
        public string SubmitTextLabel { get; set; }

        [JsonPropertyName("is_default_icon")]
        public bool IsDefaultIcon { get; set; }

        [JsonPropertyName("link_flair_position")]
        public string LinkFlairPosition { get; set; }

        [JsonPropertyName("display_name_prefixed")]
        public string DisplayNamePrefixed { get; set; }

        [JsonPropertyName("key_color")]
        public string KeyColor { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("is_default_banner")]
        public bool IsDefaultBanner { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("quarantine")]
        public bool Quarantine { get; set; }

        [JsonPropertyName("banner_size")]
        public IEnumerable<int> BannerSize { get; set; }

        [JsonPropertyName("user_is_moderator")]
        public bool UserIsModerator { get; set; }

        [JsonPropertyName("accept_followers")]
        public bool AcceptFollowers { get; set; }

        [JsonPropertyName("public_description")]
        public string PublicDescription { get; set; }

        [JsonPropertyName("link_flair_enabled")]
        public bool LinkFlairEnabled { get; set; }

        [JsonPropertyName("disable_contributor_requests")]
        public bool DisableContributorRequests { get; set; }

        [JsonPropertyName("subreddit_type")]
        public string SubredditType { get; set; }

        [JsonPropertyName("user_is_subscriber")]
        public bool UserIsSubscriber { get; set; }
    }
}
