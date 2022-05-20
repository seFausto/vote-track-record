#nullable disable
using System.Text.Json.Serialization;

namespace VotingTrackRecordClasses
{
    public class CosponsorsByParty
    {
        [JsonPropertyName("D")]
        public int? D { get; set; }

        [JsonPropertyName("R")]
        public int? R { get; set; }

        [JsonPropertyName("ID")]
        public int? ID { get; set; }
    }



}