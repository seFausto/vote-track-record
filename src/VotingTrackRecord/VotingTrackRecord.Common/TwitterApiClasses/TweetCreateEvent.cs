#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VotingTrackRecord.Common.TwitterApiClasses
{
    public class TweetCreateEvent
    {

        [JsonPropertyName("for_user_id")]
        public string ForUserId { get; set; }

        [JsonPropertyName("tweet_create_events")]
        public List<TweetObject> TweetCreateEvents { get; set; }



    }
}
