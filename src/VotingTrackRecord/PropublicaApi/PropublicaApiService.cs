﻿#nullable disable
using Microsoft.Extensions.Options;
using Refit;
using Serilog;
using VotingTrackRecord.Common.Settings;
using VotingTrackRecord.Common.PropublicaApiClasses;
using Repository;


namespace VotingTrackRecordClasses
{
    public interface IPropublicaApiService
    {
        Task<RecentVotes> GetRecentVotesAsync(string chamber);
        Task<BillSearch> SeachBills(string query);
        Task<Member> GetMemberByNameAsync(string userName, string name);
    }

    public class PropublicaApiService : IPropublicaApiService
    {
        private readonly PropublicaSettings propublicaSettings;
        private readonly IPropublicaRepository propublicaRepository;

        public PropublicaApiService(IOptions<PropublicaSettings> options, IPropublicaRepository propublicaRepository)
        {
            this.propublicaRepository = propublicaRepository;
            propublicaSettings = options.Value;
        }

        public async Task<Member> GetMemberByNameAsync(string userName, string name)
        {
            var apiService = RestService.For<IPropublicaApi>(propublicaSettings.Url);
            var chambers = new List<string>() { "house", "senate" };

            try
            {
                foreach (var chamber in chambers)
                {
                    var members = await apiService.GetMembersAsync(chamber, propublicaSettings.ApiKey);

                    var result = members.Results?.FirstOrDefault().Members?.FirstOrDefault(m =>
                            m.FirstName == name.Split(' ').First() && m.LastName == name.Split(' ').Skip(1).First());

                    if (result != null)
                    {
                        result.Chamber = members.Results?.FirstOrDefault().Chamber;
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting member by name");
                throw;
            }


            Log.Information("Member not found {Name}");
            return null;
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
        [Get("/{chamber}/members.json")]
        Task<MemberRoot> GetMembersAsync(string chamber, [Header("X-API-KEY")] string apiKey);

        [Get("/{chamber}/votes/recent.json")]
        Task<RecentVotes> GetRecentVotesAsync(string chamber, [Header("X-API-KEY")] string apiKey);

        [Get("/bills/search.json?query={query}")]
        Task<BillSearch> SearchBills(string query, [Header("X-API-KEY")] string apiKey);
    }



}