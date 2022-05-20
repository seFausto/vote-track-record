#nullable disable
using System.Text.Json.Serialization;

namespace VotingTrackRecordClasses
{
    public class Vote
    {
        [JsonPropertyName("congress")]
        public int Congress { get; set; }

        [JsonPropertyName("chamber")]
        public string Chamber { get; set; }

        [JsonPropertyName("session")]
        public int Session { get; set; }

        [JsonPropertyName("roll_call")]
        public int RollCall { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("vote_uri")]
        public string VoteUri { get; set; }

        [JsonPropertyName("bill")]
        public Bill Bill { get; set; }

        [JsonPropertyName("amendment")]
        public Amendment Amendment { get; set; }

        [JsonPropertyName("question")]
        public string Question { get; set; }

        [JsonPropertyName("question_text")]
        public string QuestionText { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("vote_type")]
        public string VoteType { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("result")]
        public string Result { get; set; }

        [JsonPropertyName("democratic")]
        public Democratic Democratic { get; set; }

        [JsonPropertyName("republican")]
        public Republican Republican { get; set; }

        [JsonPropertyName("independent")]
        public Independent Independent { get; set; }

        [JsonPropertyName("total")]
        public Total Total { get; set; }
    }



}