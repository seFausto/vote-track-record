#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VotingTrackRecord.Common.PropublicaApiClasses
{
    
    public class Amendment
    {
    }

    public class Bill
    {
        [JsonPropertyName("bill_id")]
        public string BillId { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("api_uri")]
        public string ApiUri { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("short_title")]
        public string ShortTitle { get; set; }

        [JsonPropertyName("latest_action")]
        public string LatestAction { get; set; }
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

    public class Position
    {
        [JsonPropertyName("member_id")]
        public string MemberId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("party")]
        public string Party { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("district")]
        public string District { get; set; }

        [JsonPropertyName("cook_pvi")]
        public object CookPvi { get; set; }

        [JsonPropertyName("vote_position")]
        public string VotePosition { get; set; }

        [JsonPropertyName("dw_nominate")]
        public double? DwNominate { get; set; }
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

    public class Results
    {
        [JsonPropertyName("votes")]
        public Votes Votes { get; set; }
    }

    public class VoteRoot
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("copyright")]
        public string Copyright { get; set; }

        [JsonPropertyName("results")]
        public Results Results { get; set; }
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

    public class VacantSeat
    {
        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("district")]
        public string District { get; set; }
    }

    public class Vote
    {
        [JsonPropertyName("congress")]
        public int Congress { get; set; }

        [JsonPropertyName("session")]
        public int Session { get; set; }

        [JsonPropertyName("chamber")]
        public string Chamber { get; set; }

        [JsonPropertyName("roll_call")]
        public int RollCall { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

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

        [JsonPropertyName("positions")]
        public List<Position> Positions { get; set; }
    }

    public class Votes
    {
        [JsonPropertyName("vote")]
        public Vote Vote { get; set; }

        [JsonPropertyName("vacant_seats")]
        public List<VacantSeat> VacantSeats { get; set; }
    }


}
