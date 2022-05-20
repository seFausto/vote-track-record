#nullable disable
using System.Text.Json.Serialization;

namespace VotingTrackRecordClasses
{
    public class RecentVotesResults
    {
        [JsonPropertyName("chamber")]
        public string Chamber { get; set; }

        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("num_results")]
        public int NumResults { get; set; }

        [JsonPropertyName("votes")]
        public List<Vote> Votes { get; set; }
    }



}