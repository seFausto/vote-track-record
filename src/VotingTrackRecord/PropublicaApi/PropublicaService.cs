#nullable disable
using Microsoft.Extensions.Options;
using Refit;
using Serilog;
using VotingTrackRecord.Common.Settings;
using VotingTrackRecord.Common.PropublicaApiClasses;

namespace VotingTrackRecordClasses
{
    public interface IPropublicaService
    {
        Task<RecentVotes> GetRecentVotesAsync(string chamber);
        Task<BillSearch> SeachBills(string query);
        Task<Member> GetMemberByName(string userName);
    }

    public class PropublicaService : IPropublicaService
    {
        readonly PropublicaSettings propublicaSettings;

        public PropublicaService(IOptions<PropublicaSettings> options)
        {
            propublicaSettings = options.Value;
        }

        public async Task<Member> GetMemberByName(string userName)
        {
            var apiService = RestService.For<IPropublicaApi>(propublicaSettings.Url);
            var chambers = new List<string>() { "house", "senate" };
            
            try
            {
                foreach (var chamber in chambers)
                {
                    var members = await apiService.GetMembersAsync(propublicaSettings.Congress, chamber, propublicaSettings.ApiKey);

                    var result = members.Results?.FirstOrDefault().Members?.FirstOrDefault(m => m.TwitterAccount == userName);

                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return new Member();
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
                Log.Error(ex, "Error getting recent votes");
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
                Log.Error(ex, "Error in Search bills");
                throw;
            }

        }
    }

    public interface IPropublicaApi
    {
        [Get("{congress}/{chamber}/members.json")]
        Task<MemberRoot> GetMembersAsync(string congress, string chamber, [Header("X-API-KEY")] string apiKey);
        
        [Get("/{chamber}/votes/recent.json")]
        Task<RecentVotes> GetRecentVotesAsync(string chamber, [Header("X-API-KEY")] string apiKey);
        
        [Get("/bills/search.json?query={query}")]
        Task<BillSearch> SearchBills(string query, [Header("X-API-KEY")] string apiKey);
    }



}