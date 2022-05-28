using Repository;
using System.Collections.Generic;
using VotingTrackRecordClasses;
using System.Text.Json;
using VotingTrackRecord.Common.PropublicaApiClasses;
using Serilog;

namespace VoteTracker
{
    public interface IPropublicaBusiness
    {
        Task<VotesHistory?> GetVotesHistoryAsync(Member name, IEnumerable<string> keywords);

        Task ProcessTweet(string userName, string firstName, string lastnName, string tweetText);
    }

    public class PropublicaBusiness : IPropublicaBusiness
    {
        private readonly IPropublicaApiService propublicaApiService;
        private readonly IPropublicaRepository propublicaRepository;

        public PropublicaBusiness(IPropublicaApiService propublicaService, IPropublicaRepository propublicaRepository)
        {
            this.propublicaApiService = propublicaService;
            this.propublicaRepository = propublicaRepository;
        }

        public async Task ProcessTweet(string userName, string firstName, string lastnName, string tweetText)

        {
            var member = await GetPropublicaMemberInformation(userName, firstName);

            var keywords = await GetKeywords(tweetText);

            var votesHistory = await GetVotesHistoryAsync(member, keywords);

            //await votingTrackRecordRepository.SaveVotesHistoryAsync(votesHistory);
        }

        private async Task<IEnumerable<string>> GetKeywords(string tweetText)
        {
            return new List<string> { "veteran", "gun" };
        }

        private async Task<Member> GetPropublicaMemberInformation(string userName, string name)
        {
            Log.Information("Getting member information for {userName} from mongodb", userName);
            var member = await propublicaRepository.GetMemberAsync(userName);

            if (member is null)
            {
                Log.Information("UserName {userName} not found in mongo, getting from api", userName);

                member = await propublicaApiService.GetMemberByNameAsync(userName, name);

                Log.Information("UserName {userName} member information begin saved to mongodb", userName);

                await propublicaRepository.AddMemberAsync(userName, JsonSerializer.Serialize(member));
            }

            return member;
        }


        public async Task<VotesHistory?> GetVotesHistoryAsync(Member member, IEnumerable<string> keywords)
        {
            var recentVotes = await propublicaApiService.GetRecentVotesAsync(member.Chamber);

            var results = await propublicaApiService.SeachBills(keywords.First());

            return null;
        }
    }

    public class VotesHistory
    {
    }
}