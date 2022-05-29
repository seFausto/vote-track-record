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
            return new List<string> { "fuel" };
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

            var votes = new List<VoteRoot>();
            foreach (var uri in relatedVoteUris)
            {
                votes.Add(await GetPropublicaVoteRecordAsync(uri, member));
            }

            var result = string.Empty;
            foreach (var item in votes)
            {
                var votesByMember = item?.Results?.Votes?.Vote?.Positions?.SingleOrDefault(x => x.MemberId == member.Id);
                result += $"On Bill {item?.Results.Votes.Vote.Bill.Number} - {item?.Results.Votes.Vote.Bill.ShortTitle} you voted {votesByMember?.VotePosition}\n";
            }

            return result;
        }

        private async Task<VoteRoot> GetPropublicaVoteRecordAsync(string uri, Member member)
        {
            Log.Information("Getting vote record for uri {Uri} from mongodb", uri);

            var voteRecord = await propublicaRepository.GetVoteRecordAsync(uri);

            if (voteRecord is null)
            {
                Log.Information("Vote record for uri {Uri} not found in mongo, getting from api", uri);

                voteRecord = await propublicaApiService.GetVoteRecordAsync(uri);

                Log.Information("Vote record for uri {Uri} begin saved to mongodb", uri);

                await propublicaRepository.AddVoteRecordAsync(uri, JsonSerializer.Serialize(voteRecord));
            }

            return voteRecord;
        }

    }
}