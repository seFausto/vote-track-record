using Repository;
using System.Collections.Generic;
using VotingTrackRecordClasses;
using System.Text.Json;
using VotingTrackRecord.Common.PropublicaApiClasses;
using Serilog;
using System.Linq;

namespace VoteTracker
{
    public interface IPropublicaBusiness
    {
        Task<string> GetVotesHistoryAsync(Member name, IEnumerable<string> keywords);

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
            var member = await GetPropublicaMemberInformationAsync(userName, firstName);

            var keywords = await GetKeywords(tweetText);

            var votesHistory = await GetVotesHistoryAsync(member, keywords);

            //await votingTrackRecordRepository.SaveVotesHistoryAsync(votesHistory);
        }

        private async Task<IEnumerable<string>> GetKeywords(string tweetText)
        {
            return new List<string> { "fuel", "gun" };
        }

        private async Task<Member> GetPropublicaMemberInformationAsync(string userName, string name)
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


        public async Task<string> GetVotesHistoryAsync(Member member, IEnumerable<string> keywords)
        {
            Log.Information("Getting recent votes for chamber {Chamber}", member.Chamber);

            var recentVotes = await propublicaApiService.GetRecentVotesAsync(member.Chamber);

            Log.Information("Parsing descriptions for keywords {Keywords} to get Vote Uris", keywords);

            var relatedVoteUris = (recentVotes.Results.Votes.Where(item => item.Description
                            .Contains(keywords.First(), StringComparison.InvariantCultureIgnoreCase)))
                            .Select(x => x.VoteUri)
                            .ToList();

            foreach (var uri in relatedVoteUris)
            {
                var votes = await GetPropublicaVoteRecordAsync(uri, member);
            }
            
            return null;
        }

        private async Task<IEnumerable<Vote>> GetPropublicaVoteRecordAsync(string uri, Member member)
        {
            return null;
        }

    }
}