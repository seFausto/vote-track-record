using Microsoft.Extensions.Options;
using Refit;
using System.Text.Json.Serialization;
using VotingTrackRecord;

namespace Propublica
{
    public interface IPropublicaService
    {
        Task<string> GetRecentVotesAsync(string chamber);
    }

    public class PropublicaService : IPropublicaService
    {
        PropublicaSettings propublicaSettings;

        public PropublicaService(IOptions<PropublicaSettings> options)
        {
            propublicaSettings = options.Value;
        }

        public async Task<string> GetRecentVotesAsync(string chamber)
        {
            var apiService = RestService.For<IPropublicaApi>(propublicaSettings.Url);
            try
            {
                var result = await apiService.GetRecentVotesAsync(chamber, propublicaSettings.ApiKey);

                return null;
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
        Task<Root> GetRecentVotesAsync(string chamber, [Header("X-API-KEY")] string apiKey);
    }
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
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

    public class Results
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

    public class Root
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