#nullable disable
using System.Text.Json.Serialization;

namespace VotingTrackRecordClasses
{
    public class BillSearch
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("copyright")]
        public string Copyright { get; set; }

        [JsonPropertyName("results")]
        public List<BillSearchResult> Results { get; set; }
    }



}