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
        Task<IEnumerable<string>> GetVotesHistoryAsync(Member name, IEnumerable<WordReference> keywords);

        Task ProcessTweet(string userName, string firstName, string lastnName, string tweetText);
    }

    public class PropublicaBusiness : IPropublicaBusiness
    {
        private readonly IPropublicaApiService propublicaApiService;
        private readonly IPropublicaRepository propublicaRepository;
        private readonly IWordListRepository wordListRepository;

        public PropublicaBusiness(IPropublicaApiService propublicaService, IPropublicaRepository propublicaRepository,
            IWordListRepository wordListRepository)
        {
            this.propublicaApiService = propublicaService;
            this.propublicaRepository = propublicaRepository;
            this.wordListRepository = wordListRepository;
        }

        public async Task ProcessTweet(string userName, string firstName, string lastnName, string tweetText)

        {
            var member = await GetPropublicaMemberInformationAsync(userName, firstName);

            var keywords = await GetKeywords(tweetText);
            
            var votesHistory = await GetVotesHistoryAsync(member, keywords);

            //await votingTrackRecordRepository.SaveVotesHistoryAsync(votesHistory);
        }

        private async Task<IEnumerable<WordReference>> GetKeywords(string tweetText)
        {
            var wordReferences = await wordListRepository.GetWordReferences();
            return wordReferences?.WordReferences?.Where(item =>
                        item.Related.Any(x =>
                            tweetText.ToLowerInvariant().Contains(x))).ToList() ?? new List<WordReference>();

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


        public async Task<IEnumerable<string>> GetVotesHistoryAsync(Member member, IEnumerable<WordReference> keywords)
        {
            Log.Information("Getting recent votes for chamber {Chamber}", member.Chamber);

            var recentVotes = await propublicaApiService.GetRecentVotesAsync(member.Chamber);

            Log.Information("Parsing descriptions for keywords {Keywords} to get Vote Uris", keywords);

            var keyword = keywords?.FirstOrDefault()?.Word ?? string.Empty;

            var relatedVoteUris = (recentVotes.Results.Votes.Where(item => item.Description
                            .Contains(keyword, StringComparison.InvariantCultureIgnoreCase)))
                            .Select(x => x.VoteUri)
                            .ToList();

            var voteRoots = new List<VoteRoot>();
            foreach (var uri in relatedVoteUris)
            {
                voteRoots.Add(await GetPropublicaVoteRecordAsync(uri));
            }

            var result = new List<string>();
            foreach (var item in voteRoots)
            {
                var memberPositions = item.Results.Votes.Vote.Positions.SingleOrDefault(x => x.MemberId == member.Id);

                if (memberPositions is null)
                    continue;

                var billIdTitle = $"{item.Results.Votes.Vote.Bill.BillId} - {item.Results.Votes.Vote.Bill.Title}";

                result.Add($"You Voted {memberPositions.VotePosition.ToUpper()} on {billIdTitle} \n");
            }


            result.ForEach(x => Log.Debug(x));

            return result;
        }

        private async Task<VoteRoot> GetPropublicaVoteRecordAsync(string uri)
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