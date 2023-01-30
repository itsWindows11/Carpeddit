using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Carpeddit.App.ViewModels
{
    public sealed partial class RedditPrefsViewModel : ObservableObject
    {
        [ObservableProperty]
        [property: JsonPropertyName("accept_pms")]
        private string acceptPms;

        [ObservableProperty]
        [property: JsonPropertyName("bad_comment_autocollapse")]
        private string badCommentAutocollapse;

        [ObservableProperty]
        [property: JsonPropertyName("beta")]
        private bool beta;

        [ObservableProperty]
        [property: JsonPropertyName("collapse_read_messages")]
        private bool collapseReadMessages;

        [ObservableProperty]
        [property: JsonPropertyName("compress")]
        private bool compress;

        [ObservableProperty]
        [property: JsonPropertyName("country_code")]
        private string countryCode;

        [ObservableProperty]
        [property: JsonPropertyName("default_comment_sort")]
        private string defaultCommentSort;

        [ObservableProperty]
        [property: JsonPropertyName("email_chat_request")]
        private bool emailChatRequest;

        [ObservableProperty]
        [property: JsonPropertyName("email_comment_reply")]
        private bool emailCommentReply;

        [ObservableProperty]
        [property: JsonPropertyName("email_community_discovery")]
        private bool emailCommunityDiscovery;

        [ObservableProperty]
        [property: JsonPropertyName("email_digests")]
        private bool emailDigests;

        [ObservableProperty]
        [property: JsonPropertyName("email_messages")]
        private bool emailMessages;

        [ObservableProperty]
        [property: JsonPropertyName("email_new_user_welcome")]
        private bool emailNewUserWelcome;

        [ObservableProperty]
        [property: JsonPropertyName("email_post_reply")]
        private bool emailPostReply;

        [ObservableProperty]
        [property: JsonPropertyName("email_private_message")]
        private bool emailPrivateMessage;

        [ObservableProperty]
        [property: JsonPropertyName("email_unsubscribe_all")]
        private bool emailUnsubscribeAll;

        [ObservableProperty]
        [property: JsonPropertyName("email_upvote_comment")]
        private bool emailUpvoteComment;

        [ObservableProperty]
        [property: JsonPropertyName("email_upvote_post")]
        private bool emailUpvotePost;

        [ObservableProperty]
        [property: JsonPropertyName("email_user_new_follower")]
        private bool emailUserNewFollower;

        [ObservableProperty]
        [property: JsonPropertyName("email_username_mention")]
        private bool emailUsernameMention;

        [ObservableProperty]
        [property: JsonPropertyName("enable_followers")]
        private bool enableFollowers;

        [ObservableProperty]
        [property: JsonPropertyName("ignore_suggested_sort")]
        private bool ignoreSuggestedSort;

        [ObservableProperty]
        [property: JsonPropertyName("label_nsfw")]
        private bool labelNsfw;

        [ObservableProperty]
        [property: JsonPropertyName("lang")]
        private string lang;

        [ObservableProperty]
        [property: JsonPropertyName("legacy_search")]
        private bool legacySearch;

        [ObservableProperty]
        [property: JsonPropertyName("mark_messages_read")]
        private bool markMessagesRead;

        [ObservableProperty]
        [property: JsonPropertyName("media")]
        private string media;

        [ObservableProperty]
        [property: JsonPropertyName("media_preview")]
        private string mediaPreview;

        [ObservableProperty]
        [property: JsonPropertyName("num_comments")]
        private int numComments;

        [ObservableProperty]
        [property: JsonPropertyName("search_include_over_18")]
        private bool searchIncludeOver18;

        [ObservableProperty]
        [property: JsonPropertyName("send_crosspost_messages")]
        private bool sendCrosspostMessages;

        [ObservableProperty]
        [property: JsonPropertyName("send_welcome_messages")]
        private bool sendWelcomeMessages;

        [ObservableProperty]
        [property: JsonPropertyName("show_flair")]
        private bool showFlair;

        [ObservableProperty]
        [property: JsonPropertyName("show_link_flair")]
        private bool showLinkFlair;

        [ObservableProperty]
        [property: JsonPropertyName("show_location_based_recommendations")]
        private bool showLocationBasedRecommendations;

        [ObservableProperty]
        [property: JsonPropertyName("show_presence")]
        private bool showPresence;

        [ObservableProperty]
        [property: JsonPropertyName("show_trending")]
        private bool showTrending;

        [ObservableProperty]
        [property: JsonPropertyName("video_autoplay")]
        private bool videoAutoplay;

        public async Task UpdatePrefsAsync()
        {

        }
    }
}
