#nullable disable
using Microsoft.Extensions.Options;
using Refit;
using Serilog;
using VotingTrackRecord.Common.Settings;

namespace VotingTrackRecordClasses
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
        [Get("/{chamber}/votes/recent.json")]
        Task<RecentVotes> GetRecentVotesAsync(string chamber, [Header("X-API-KEY")] string apiKey);
        
        [Get("/bills/search.json?query={query}")]
        Task<BillSearch> SearchBills(string query, [Header("X-API-KEY")] string apiKey);
    }



}