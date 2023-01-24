using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Carpeddit.App.Api.Models
{
    public sealed class VotingInput
    {
        /// <summary>
        /// Voting direction. 1 to upvote, -1 to downvote, 0 to unvote.
        /// </summary>
        /// <remarks>Note: votes must be cast by humans. That is, API clients proxying a human's action one-for-one are OK, but bots deciding how to vote on content or amplifying a human's vote are not. See the reddit rules for more details on what constitutes vote cheating.</remarks>
        [AliasAs("dir")]
        [JsonPropertyName("dir")]
        public int Direction { get; set; }

        /// <summary>
        /// Fullname of a votable.
        /// </summary>
        [AliasAs("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        public VotingInput(int direction, string id)
        {
            Direction = direction;
            Id = id;
        }
    }
}
