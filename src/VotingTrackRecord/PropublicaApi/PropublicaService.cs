#nullable disable
using Microsoft.Extensions.Options;
using Refit;
using System.Text.Json.Serialization;
using VotingTrackRecord;

namespace Propublica
{
    public interface IPropublicaService
    {
        Task<RecentVotes> GetRecentVotesAsync(string chamber);
        Task<BillSearch> SeachBills(string query);
    }

    public class PropublicaService : IPropublicaService
    {
        readonly PropublicaSettings propublicaSettings;

        public PropublicaService(IOptions<PropublicaSettings> options)
        {
            propublicaSettings = options.Value;
        }

        public async Task<RecentVotes> GetRecentVotesAsync(string chamber)
        {
            var apiService = RestService.For<IPropublicaApi>(propublicaSettings.Url);
            try
            {
                var result = await apiService.GetRecentVotesAsync(chamber, propublicaSettings.ApiKey);

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<BillSearch> SeachBills(string query)
        {
            var apiService = RestService.For<IPropublicaApi>(propublicaSettings.Url);
            try
            {
                var result = await apiService.SearchBills(query, propublicaSettings.ApiKey);

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }

    public interface IPropublicaApi
    {
        [Get("/{chamber}/votes/recent.json")]
        Task<RecentVotes> GetRecentVotesAsync(string chamber, [Header("X-API-KEY")] string apiKey);
        [Get("/bills/search.json?query={query}")]
        Task<BillSearch> SearchBills(string query, [Header("X-API-KEY")] string apiKey);
    }

    public class Amendment
    {
    }

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

    public class RecentVotesResults
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

    public class RecentVotes
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("copyright")]
        public string Copyright { get; set; }

        [JsonPropertyName("results")]
        public RecentVotesResults Results { get; set; }
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

     public class CosponsorsByParty
    {
        [JsonPropertyName("D")]
        public int? D { get; set; }

        [JsonPropertyName("R")]
        public int? R { get; set; }

        [JsonPropertyName("ID")]
        public int? ID { get; set; }
    }

    public class BillSearchResult
    {
        [JsonPropertyName("num_results")]
        public int NumResults { get; set; }

        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("bills")]
        public List<Bill> Bills { get; set; }
    }

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