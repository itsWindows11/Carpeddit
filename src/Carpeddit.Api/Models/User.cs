using Carpeddit.Common.Converters;
using System;
using System.Collections.Generic;
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
        public int InboxCount { get; set; }

        [JsonPropertyName("comment_karma")]
        public int CommentKarma { get; set; }

        [JsonPropertyName("awardee_karma")]
        public int AwardeeKarma { get; set; }

        [JsonPropertyName("total_karma")]
        public int TotalKarma { get; set; }

        [JsonPropertyName("awarder_karma")]
        public int AwarderKarma { get; set; }

        [JsonPropertyName("has_mail")]
        public bool? HasMail { get; set; }

        [JsonPropertyName("has_mod_mail")]
        public bool? HasModMail { get; set; }

        [JsonPropertyName("has_verified_email")]
        public bool HasVerifiedEmail { get; set; }

        [JsonPropertyName("has_subscribed_to_premium")]
        public bool HasSubscribedToPremium { get; set; }

        [JsonPropertyName("has_external_account")]
        public bool HasExternalAccount { get; set; }

        [JsonPropertyName("is_employee")]
        public bool IsEmployee { get; set; }

        [JsonPropertyName("subreddit")]
        public UserSubreddit Subreddit { get; set; }

        [JsonPropertyName("snoovatar_img")]
        public string SnooavatarImage { get; set; }

        [JsonPropertyName("snoovatar_size")]
        public int[] SnooavatarSizes { get; set; }

        [JsonPropertyName("gold_expiration")]
        public object GoldExpiration { get; set; }

        [JsonPropertyName("has_gold_subscription")]
        public bool HasGoldSubscription { get; set; }

        [JsonPropertyName("is_sponsor")]
        public bool IsSponsor { get; set; }

        [JsonPropertyName("num_friends")]
        public int NumFriends { get; set; }

        [JsonPropertyName("can_edit_name")]
        public bool CanEditName { get; set; }

        [JsonPropertyName("verified")]
        public bool Verified { get; set; }

        [JsonPropertyName("new_modmail_exists")]
        public bool NewModmailExists { get; set; }

        [JsonPropertyName("coins")]
        public int Coins { get; set; }

        [JsonPropertyName("has_paypal_subscription")]
        public bool HasPaypalSubscription { get; set; }

        [JsonPropertyName("has_stripe_subscription")]
        public bool HasStripeSubscription { get; set; }

        [JsonPropertyName("can_create_subreddit")]
        public bool CanCreateSubreddit { get; set; }

        [JsonPropertyName("is_gold")]
        public bool IsGold { get; set; }

        [JsonPropertyName("suspension_expiration_utc")]
        public int? SuspensionExpirationUTC { get; set; }

        [JsonPropertyName("is_suspended")]
        public bool IsSuspended { get; set; }

        [JsonPropertyName("in_redesign_beta")]
        public bool InRedesignBeta { get; set; }

        [JsonPropertyName("icon_img")]
        public string IconImage { get; set; }

        [JsonPropertyName("pref_nightmode")]
        public bool NightMode { get; set; }

        [JsonPropertyName("hide_from_robots")]
        public bool HideFromRobots { get; set; }

        [JsonPropertyName("password_set")]
        public bool? PasswordSet { get; set; }

        [JsonPropertyName("pref_show_snoovatar")]
        public bool PrefShowSnooavatar { get; set; }

        [JsonPropertyName("created")]
        [JsonConverter(typeof(LocalTimestampConverter))]
        public DateTime Created { get; set; }

        [JsonPropertyName("gold_creddits")]
        public int GoldCreddits { get; set; }

        [JsonPropertyName("created_utc")]
        [JsonConverter(typeof(UtcTimestampConverter))]
        public DateTime CreatedUtc { get; set; }

        [JsonPropertyName("pref_show_twitter")]
        public bool PrefShowTwitter { get; set; }

        [JsonPropertyName("in_beta")]
        public bool InBeta { get; set; }

        [JsonPropertyName("accept_followers")]
        public bool AcceptFollowers { get; set; }

        [JsonPropertyName("has_subscribed")]
        public bool HasSubscribed { get; set; }

        [JsonPropertyName("linked_identities")]
        public IEnumerable<string> LinkedIdentities { get; set; }
    }
}
