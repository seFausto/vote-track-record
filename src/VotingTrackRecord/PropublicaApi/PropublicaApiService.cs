#nullable disable
using Microsoft.Extensions.Options;
using Refit;
using Serilog;
using VotingTrackRecord.Common.Settings;
using VotingTrackRecord.Common.PropublicaApiClasses;
using Repository;
using System.Text.RegularExpressions;
using Extensions;

namespace VotingTrackRecordClasses
{
    public interface IPropublicaApiService
    {
        Task<RecentVotesRoot> GetRecentVotesAsync(string chamber, int pageNumber = 0);
        Task<BillSearchRoot> SearchBills(string query);
        Task<Member> GetMemberByNameAsync(string name, string screenName);
        Task<VoteRoot> GetVoteRecordAsync(string uri);
    }

    public class PropublicaApiService : IPropublicaApiService
    {
        private readonly PropublicaSettings propublicaSettings;


        public PropublicaApiService(IOptions<PropublicaSettings> options)
        {
            propublicaSettings = options.Value;
        }

        public async Task<Member> GetMemberByNameAsync(string name, string screenName)
        {
            var apiUrl = propublicaSettings.Url.Combine(propublicaSettings.Congress);
            var apiService = RestService.For<IPropublicaApi>(apiUrl);

            var chambers = new List<string>() { "house", "senate" };

            var cleanName = CleanUpName(name);

            try
            {
                foreach (var chamber in chambers)
                {
                    var members = await apiService.GetMembersAsync(chamber, propublicaSettings.ApiKey);
                    
                    var cleanNameArray = cleanName.Split(' ');

                    var result = members.Results?.FirstOrDefault().Members?.FirstOrDefault(m =>
                            (m.FirstName == cleanNameArray.First() && m.LastName == cleanNameArray.Last()) ||
                            m.TwitterAccount == screenName);

                    if (result != null)
                    {
                        result.Chamber = members.Results?.FirstOrDefault().Chamber;
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting member by name: {Name}", cleanName);
                throw;
            }

            Log.Information("Member not found {Name}", cleanName);
            return null;
        }

        private static string CleanUpName(string name)
        {
            name = name.Replace("Rep.", string.Empty).Replace("Sen.", string.Empty);
            name = name.Replace("Senator", string.Empty);

            name = Regex.Replace(name, @"[^\u0000-\u007F]+", string.Empty);
            name = name.Trim();
            return name;
        }

        public async Task<RecentVotesRoot> GetRecentVotesAsync(string chamber, int pageNumber = 0)
        {
            var apiService = RestService.For<IPropublicaApi>(propublicaSettings.Url);
            try
            {
                var parameters = new RecentVotesParameters()
                {
                    Offset = pageNumber * propublicaSettings.PageSize,
                };

                return await apiService.GetRecentVotesAsync(chamber, parameters, propublicaSettings.ApiKey);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting recent votes for chamber {Chamber}", chamber);
                throw;
            }

        }

        public async Task<BillSearchRoot> SearchBills(string query)
        {
            var apiUrl = propublicaSettings.Url.Combine(propublicaSettings.Congress);
            var apiService = RestService.For<IPropublicaApi>(apiUrl);

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

        public async Task<VoteRoot> GetVoteRecordAsync(string uri)
        {
            var apiService = RestService.For<IPropublicaApi>(uri);

            try
            {
                return await apiService.GetVoteRecordAsync(propublicaSettings.ApiKey);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in Search bills {Uri}", uri);
                throw;
            }
        }
    }

    public class RecentVotesParameters
    {
        [AliasAs("offset")]
        public int Offset { get; set; }
    }

    public interface IPropublicaApi
    {
        [Get("/{chamber}/members.json")]
        Task<MemberRoot> GetMembersAsync(string chamber, [Header("X-API-KEY")] string apiKey);

        [Get("/{chamber}/votes/recent.json")]
        Task<RecentVotesRoot> GetRecentVotesAsync(string chamber, RecentVotesParameters parameteres,
            [Header("X-API-KEY")] string apiKey);

        [Get("/bills/search.json?query={query}")]
        Task<BillSearchRoot> SearchBills(string query, [Header("X-API-KEY")] string apiKey);

        [Get("/")]
        Task<VoteRoot> GetVoteRecordAsync([Header("X-API-KEY")] string apiKey);
    }



}