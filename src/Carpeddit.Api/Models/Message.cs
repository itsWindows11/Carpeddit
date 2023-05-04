using Carpeddit.Common.Converters;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Carpeddit.Api.Models
{
    public sealed class Message
    {
        /*[JsonPropertyName("first_message")]
        public int? FirstMessage { get; set; }*/

        [JsonPropertyName("first_message_name")]
        public string FirstMessageName { get; set; }

        [JsonPropertyName("subreddit")]
        public string Subreddit { get; set; }

        [JsonPropertyName("likes")]
        public bool? Likes { get; set; }

        /*[JsonPropertyName("replies")]
        [JsonConverter(typeof(EmptyStringToListConverter<Message>))]
        public IEnumerable<Message> Replies { get; set; }*/

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        [JsonPropertyName("was_comment")]
        public bool WasComment { get; set; }

        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("num_comments")]
        public int? NumComments { get; set; }

        [JsonPropertyName("parent_id")]
        public string ParentId { get; set; }

        [JsonPropertyName("subreddit_name_prefixed")]
        public string SubredditNamePrefixed { get; set; }

        [JsonPropertyName("new")]
        public bool New { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }

        [JsonPropertyName("dest")]
        public string Dest { get; set; }

        [JsonPropertyName("body_html")]
        public string BodyHTML { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("created")]
        [JsonConverter(typeof(LocalTimestampConverter))]
        public DateTime Created { get; set; }

        [JsonPropertyName("created_utc")]
        [JsonConverter(typeof(UtcTimestampConverter))]
        public DateTime CreatedUTC { get; set; }

        [JsonPropertyName("context")]
        public string Context { get; set; }

        [JsonPropertyName("distinguished")]
        public string Distinguished { get; set; }
    }
}
