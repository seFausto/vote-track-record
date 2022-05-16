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
            
            var result = await apiService.GetRecentVotesAsync(chamber, propublicaSettings.ApiKey);

            return null;
        }
    }

    public interface IPropublicaApi
    {
        [Get("/{chamber}/votes/recent.json")]
        Task<Root> GetRecentVotesAsync(string chamber, [Header("X-API-KEY")] string apiKey);
    }

    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class Root
    {
        [JsonPropertyName("status")]
        public string Status;

        [JsonPropertyName("copyright")]
        public string Copyright;
              
        public Results Results;
    }

    public class Amendment
    {
    }

    public class Bill
    {
        [JsonPropertyName("bill_id")]
        public string BillId;

        [JsonPropertyName("number")]
        public string Number;

        [JsonPropertyName("sponsor_id")]
        public string SponsorId;

        [JsonPropertyName("api_uri")]
        public string ApiUri;

        [JsonPropertyName("title")]
        public string Title;

        [JsonPropertyName("latest_action")]
        public string LatestAction;
    }

    public class Democratic
    {
        [JsonPropertyName("yes")]
        public int Yes;

        [JsonPropertyName("no")]
        public int No;

        [JsonPropertyName("present")]
        public int Present;

        [JsonPropertyName("not_voting")]
        public int NotVoting;

        [JsonPropertyName("majority_position")]
        public string MajorityPosition;
    }

    public class Independent
    {
        [JsonPropertyName("yes")]
        public int Yes;

        [JsonPropertyName("no")]
        public int No;

        [JsonPropertyName("present")]
        public int Present;

        [JsonPropertyName("not_voting")]
        public int NotVoting;
    }

    public class Republican
    {
        [JsonPropertyName("yes")]
        public int Yes;

        [JsonPropertyName("no")]
        public int No;

        [JsonPropertyName("present")]
        public int Present;

        [JsonPropertyName("not_voting")]
        public int NotVoting;

        [JsonPropertyName("majority_position")]
        public string MajorityPosition;
    }

    public class Results
    {
        [JsonPropertyName("chamber")]
        public string Chamber;

        [JsonPropertyName("offset")]
        public int Offset;

        [JsonPropertyName("num_results")]
        public int NumResults;

        [JsonPropertyName("votes")]
        public List<Vote> Votes;
    }
    public class Total
    {
        [JsonPropertyName("yes")]
        public int Yes;

        [JsonPropertyName("no")]
        public int No;

        [JsonPropertyName("present")]
        public int Present;

        [JsonPropertyName("not_voting")]
        public int NotVoting;
    }

    public class Vote
    {
        [JsonPropertyName("congress")]
        public int Congress;

        [JsonPropertyName("chamber")]
        public string Chamber;

        [JsonPropertyName("session")]
        public int Session;

        [JsonPropertyName("roll_call")]
        public int RollCall;

        [JsonPropertyName("source")]
        public string Source;

        [JsonPropertyName("url")]
        public string Url;

        [JsonPropertyName("vote_uri")]
        public string VoteUri;

        [JsonPropertyName("bill")]
        public Bill Bill;

        [JsonPropertyName("amendment")]
        public Amendment Amendment;

        [JsonPropertyName("question")]
        public string Question;

        [JsonPropertyName("question_text")]
        public string QuestionText;

        [JsonPropertyName("description")]
        public string Description;

        [JsonPropertyName("vote_type")]
        public string VoteType;

        [JsonPropertyName("date")]
        public string Date;

        [JsonPropertyName("time")]
        public string Time;

        [JsonPropertyName("result")]
        public string Result;

        [JsonPropertyName("democratic")]
        public Democratic Democratic;

        [JsonPropertyName("republican")]
        public Republican Republican;

        [JsonPropertyName("independent")]
        public Independent Independent;

        [JsonPropertyName("total")]
        public Total Total;
    }



}