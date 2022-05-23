using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VotingTrackRecord.Common.TwitterApiClasses
{
    public class TweetObject
    {
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("id_str")]
        public string IdStr { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        //[JsonPropertyName("user")]
        //public User User { get; set; }

        //[JsonPropertyName("entities")]
        //public Entities Entities { get; set; }
    }
}
