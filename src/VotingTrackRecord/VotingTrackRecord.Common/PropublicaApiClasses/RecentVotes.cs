#nullable disable
using System.Text.Json.Serialization;

namespace VotingTrackRecordClasses
{
    public class RecentVotes
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("copyright")]
        public string Copyright { get; set; }

        [JsonPropertyName("results")]
        public RecentVotesResults Results { get; set; }
    }



}