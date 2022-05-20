#nullable disable
using System.Text.Json.Serialization;

namespace VotingTrackRecordClasses
{
    public class BillSearchResult
    {
        [JsonPropertyName("num_results")]
        public int NumResults { get; set; }

        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("bills")]
        public List<Bill> Bills { get; set; }
    }



}