#nullable disable
using System.Text.Json.Serialization;

namespace VotingTrackRecordClasses
{
    public class BillSearchRoot
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("copyright")]
        public string Copyright { get; set; }

        [JsonPropertyName("results")]
        public List<BillSearch> Results { get; set; }
    }

    public class BillSearch
    {
        [JsonPropertyName("num_results")]
        public int NumResults { get; set; }

        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("bills")]
        public List<Bill> Bills { get; set; }
    }

}