#nullable disable
using System.Text.Json.Serialization;

namespace VotingTrackRecordClasses
{
    public class Bill
    {
        [JsonPropertyName("bill_id")]
        public string BillId { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("sponsor_id")]
        public string SponsorId { get; set; }

        [JsonPropertyName("api_uri")]
        public string ApiUri { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("latest_action")]
        public string LatestAction { get; set; }
    }



}