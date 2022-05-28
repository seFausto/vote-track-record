#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VotingTrackRecord.Common.PropublicaApiClasses
{
    public class Member
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("short_title")]
        public string ShortTitle { get; set; }

        [JsonPropertyName("api_uri")]
        public string ApiUri { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("middle_name")]
        public string MiddleName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("suffix")]
        public string Suffix { get; set; }

        [JsonPropertyName("date_of_birth")]
        public string DateOfBirth { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("party")]
        public string Party { get; set; }

        [JsonPropertyName("leadership_role")]
        public string LeadershipRole { get; set; }

        [JsonPropertyName("twitter_account")]
        public string TwitterAccount { get; set; }

        [JsonPropertyName("facebook_account")]
        public string FacebookAccount { get; set; }

        [JsonPropertyName("youtube_account")]
        public string YoutubeAccount { get; set; }

        [JsonPropertyName("govtrack_id")]
        public string GovtrackId { get; set; }

        [JsonPropertyName("cspan_id")]
        public string CspanId { get; set; }

        [JsonPropertyName("votesmart_id")]
        public string VotesmartId { get; set; }

        [JsonPropertyName("icpsr_id")]
        public string IcpsrId { get; set; }

        [JsonPropertyName("crp_id")]
        public string CrpId { get; set; }

        [JsonPropertyName("google_entity_id")]
        public string GoogleEntityId { get; set; }

        [JsonPropertyName("fec_candidate_id")]
        public string FecCandidateId { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("rss_url")]
        public string RssUrl { get; set; }

        [JsonPropertyName("contact_form")]
        public object ContactForm { get; set; }

        [JsonPropertyName("in_office")]
        public bool InOffice { get; set; }

        [JsonPropertyName("cook_pvi")]
        public object CookPvi { get; set; }

        [JsonPropertyName("dw_nominate")]
        public double? DwNominate { get; set; }

        [JsonPropertyName("ideal_point")]
        public object IdealPoint { get; set; }

        [JsonPropertyName("seniority")]
        public string Seniority { get; set; }

        [JsonPropertyName("next_election")]
        public string NextElection { get; set; }

        [JsonPropertyName("total_votes")]
        public int? TotalVotes { get; set; }

        [JsonPropertyName("missed_votes")]
        public int? MissedVotes { get; set; }

        [JsonPropertyName("total_present")]
        public int? TotalPresent { get; set; }

        [JsonPropertyName("last_updated")]
        public string LastUpdated { get; set; }

        [JsonPropertyName("ocd_id")]
        public string OcdId { get; set; }

        [JsonPropertyName("office")]
        public string Office { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("fax")]
        public object Fax { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("district")]
        public string District { get; set; }

        [JsonPropertyName("at_large")]
        public bool AtLarge { get; set; }

        [JsonPropertyName("geoid")]
        public string Geoid { get; set; }

        [JsonPropertyName("missed_votes_pct")]
        public double MissedVotesPct { get; set; }

        [JsonPropertyName("votes_with_party_pct")]
        public double VotesWithPartyPct { get; set; }

        [JsonPropertyName("votes_against_party_pct")]
        public double VotesAgainstPartyPct { get; set; }
    }

    public class Result
    {
        [JsonPropertyName("congress")]
        public string Congress { get; set; }

        [JsonPropertyName("chamber")]
        public string Chamber { get; set; }

        [JsonPropertyName("num_results")]
        public int? NumResults { get; set; }

        [JsonPropertyName("offset")]
        public int? Offset { get; set; }

        [JsonPropertyName("members")]
        public List<Member> Members { get; set; }
    }

    public class MemberRoot
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("copyright")]
        public string Copyright { get; set; }

        [JsonPropertyName("results")]
        public List<Result> Results { get; set; }
    }


}
