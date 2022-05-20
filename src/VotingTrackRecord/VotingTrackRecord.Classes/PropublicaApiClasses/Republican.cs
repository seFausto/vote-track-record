﻿#nullable disable
using System.Text.Json.Serialization;

namespace VotingTrackRecordClasses
{
    public class Republican
    {
        [JsonPropertyName("yes")]
        public int Yes { get; set; }

        [JsonPropertyName("no")]
        public int No { get; set; }

        [JsonPropertyName("present")]
        public int Present { get; set; }

        [JsonPropertyName("not_voting")]
        public int NotVoting { get; set; }

        [JsonPropertyName("majority_position")]
        public string MajorityPosition { get; set; }
    }



}