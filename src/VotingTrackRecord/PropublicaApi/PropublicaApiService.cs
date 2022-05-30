#nullable disable
using Microsoft.Extensions.Options;
using Refit;
using Serilog;
using VotingTrackRecord.Common.Settings;
using VotingTrackRecord.Common.PropublicaApiClasses;
using Repository;
using System.Text.RegularExpressions;

namespace VotingTrackRecordClasses
{
    public interface IPropublicaApiService
    {
        Task<RecentVotesRoot> GetRecentVotesAsync(string chamber);
        Task<BillSearchRoot> SeachBills(string query);
        Task<Member> GetMemberByNameAsync(string userName, string name);
        Task<VoteRoot> GetVoteRecordAsync(string uri);
    }

    public class PropublicaApiService : IPropublicaApiService
    {
        private readonly PropublicaSettings propublicaSettings;


        public PropublicaApiService(IOptions<PropublicaSettings> options)
        {
            propublicaSettings = options.Value;
        }

        public static string Combine(string uri1, string uri2)
        {
            uri1 = uri1.TrimEnd('/');
            uri2 = uri2.TrimStart('/');
            return string.Format("{0}/{1}", uri1, uri2);
        }

        public async Task<Member> GetMemberByNameAsync(string userName, string name)
        {
            var apiService = RestService.For<IPropublicaApi>(Combine(propublicaSettings.Url, propublicaSettings.Congress));
            var chambers = new List<string>() { "house", "senate" };
            name = CleanupName(name);
            try
            {
                foreach (var chamber in chambers)
                {
                    var members = await apiService.GetMembersAsync(chamber, propublicaSettings.ApiKey);

                    var result = members.Results?.FirstOrDefault().Members?.FirstOrDefault(m =>
                            m.FirstName == name.Split(' ').First() && m.LastName == name.Split(' ').Last());

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

        private static string CleanupName(string name)
        {
            name = name.Replace("Rep.", string.Empty).Replace("Sen.", string.Empty);
            name = Regex.Replace(name, @"[^\u0000-\u007F]+", string.Empty);
            name = name.Trim();
            return name;
        }

        public async Task<RecentVotesRoot> GetRecentVotesAsync(string chamber)
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

        public async Task<BillSearchRoot> SeachBills(string query)
        {
            var apiService = RestService.For<IPropublicaApi>(Combine(propublicaSettings.Url, propublicaSettings.Congress));
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
                var result = await apiService.GetVoteRecordAsync(propublicaSettings.ApiKey);

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
        Task<RecentVotesRoot> GetRecentVotesAsync(string chamber, [Header("X-API-KEY")] string apiKey);

        [Get("/bills/search.json?query={query}")]
        Task<BillSearchRoot> SearchBills(string query, [Header("X-API-KEY")] string apiKey);

        [Get("/")]
        Task<VoteRoot> GetVoteRecordAsync([Header("X-API-KEY")] string apiKey);
    }



}