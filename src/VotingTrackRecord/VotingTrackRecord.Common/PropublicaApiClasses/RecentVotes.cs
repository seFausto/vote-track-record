#nullable disable
using System.Text.Json.Serialization;

namespace VotingTrackRecordClasses
{
    public class RecentVotesRoot
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("copyright")]
        public string Copyright { get; set; }

        [JsonPropertyName("results")]
        public RecentVotes Results { get; set; }
    }

    public class CosponsorsByParty
    {
        [JsonPropertyName("D")]
        public int? D { get; set; }

        [JsonPropertyName("R")]
        public int? R { get; set; }

        [JsonPropertyName("ID")]
        public int? ID { get; set; }
    }

    public class RecentVotes
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

    public class Total
    {
        [JsonPropertyName("yes")]
        public int Yes { get; set; }

        [JsonPropertyName("no")]
        public int No { get; set; }

        [JsonPropertyName("present")]
        public int Present { get; set; }

        [JsonPropertyName("not_voting")]
        public int NotVoting { get; set; }
    }

    public class Democratic
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

    public class Independent
    {
        [JsonPropertyName("yes")]
        public int Yes { get; set; }

        [JsonPropertyName("no")]
        public int No { get; set; }

        [JsonPropertyName("present")]
        public int Present { get; set; }

        [JsonPropertyName("not_voting")]
        public int NotVoting { get; set; }
    }

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