using System.Text.Json.Serialization;

namespace Carpeddit.Api.Models
{
    public class ListingInput
    {
        /// <summary>
        /// Fullname of a thing.
        /// </summary>
        [JsonPropertyName("after")]
        public string After { get; set; }

        /// <summary>
        /// Fullname of a thing.
        /// </summary>
        [JsonPropertyName("before")]
        public string Before { get; set; }

        /// <summary>
        /// The maximum number of items desired.
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        /// <summary>
        /// A positive integer.
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }

        /// <summary>
        /// Populate a new listing input.
        /// </summary>
        /// <param name="after">fullname of a thing</param>
        /// <param name="before">fullname of a thing</param>
        /// <param name="limit">the maximum number of items desired (default: 25)</param>
        /// <param name="count">a positive integer (default: 0)</param>
        public ListingInput(string after = "", string before = "", int limit = 25, int count = 0)
        {
            After = after;
            Before = before;
            Limit = limit;
            Count = count;
        }
    }
}
