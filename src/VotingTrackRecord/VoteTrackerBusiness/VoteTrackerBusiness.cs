using Repository;
using System.Collections.Generic;
using VotingTrackRecordClasses;
using System.Text.Json;
using VotingTrackRecord.Common.PropublicaApiClasses;
using Serilog;
using System.Linq;
using Microsoft.Extensions.Options;
using VotingTrackRecord.Common.Settings;
using Extensions;

namespace VoteTracker
{
    public interface IPropublicaBusiness
    {
        Task<IEnumerable<string>> GetReplyMessage(string userName, string name, 
            string tweetText, Member member);
        Task<Member?> GetPropublicaMemberInformationAsync(string userName, string name);
    }

    public class PropublicaBusiness : IPropublicaBusiness
    {
        private readonly IPropublicaApiService propublicaApiService;
        private readonly IPropublicaRepository propublicaRepository;
        private readonly IWordListRepository wordListRepository;
        private readonly PropublicaSettings propublicaSettings;

        private const int RecentVotePageLimit = 5;

        public PropublicaBusiness(IOptions<PropublicaSettings> settings, IPropublicaApiService propublicaApiService, 
            IPropublicaRepository propublicaRepository, IWordListRepository wordListRepository)
        {
            this.propublicaApiService = propublicaApiService;
            this.propublicaRepository = propublicaRepository;
            this.wordListRepository = wordListRepository;

            propublicaSettings = settings.Value;
        }

        public async Task<IEnumerable<string>> GetReplyMessage(string userName, string name, string tweetText, 
            Member member)
        {
            var keywords = await GetKeywordsAsync(tweetText);

            if (!keywords.HasItems())
            {
                return new List<string>();
            }

            return await GetLatestRelatedVotesMessageAsync(member, keywords);
        }

        private async Task<IEnumerable<WordReference>> GetKeywordsAsync(string tweetText)
        {
            var wordReferences = await wordListRepository.GetWordReferences();

            return wordReferences?.WordReferences?.Where(item =>
                        item.Related.Any(x =>
                            tweetText.ToLowerInvariant().Contains(x))).ToList() ?? new List<WordReference>();
        }

        public async Task<Member?> GetPropublicaMemberInformationAsync(string userName, string name)
        {
            Log.Information("Getting member information for {userName} from mongodb", userName);
            var member = await propublicaRepository.GetMemberAsync(userName);

            if (member is null)
            {
                Log.Information("UserName {userName} not found in mongo, getting from api", userName);

                member = await propublicaApiService.GetMemberByNameAsync(name, userName);

                if (member is null)
                {
                    Log.Error("UserName {userName} not found in propublica api", userName);
                    return null;
                }

                Log.Information("UserName {userName} member information begin saved to mongodb", userName);

                await propublicaRepository.AddMemberAsync(userName, JsonSerializer.Serialize(member));
            }

            return member;
        }


        private async Task<IEnumerable<string>> GetLatestRelatedVotesMessageAsync(Member member,
            IEnumerable<WordReference> keywords)
        {
            var pageNumber = 0;
            var relatedVoteUris = new List<string>();

            do
            {
                Log.Information("Getting recent votes for chamber {Chamber}, Page: {PageNumber}",
                    member.Chamber, pageNumber);

                var recentVotes = await propublicaApiService.GetRecentVotesAsync(member.Chamber, pageNumber);

                Log.Information("Parsing descriptions for keywords {Keywords} to get Vote Uris",
                    keywords.Select(x => x.Word));

                foreach (var wordReference in keywords)
                {
                    relatedVoteUris.AddRange(recentVotes.Results.Votes.Where(item =>
                                wordReference.Related.Any(x =>
                                    item.Description.Contains(x, StringComparison.CurrentCultureIgnoreCase)))
                                .Select(x => x.VoteUri).ToList());
                }

                pageNumber++;
            } while (pageNumber <= RecentVotePageLimit && !relatedVoteUris.HasItems());

            var voteRoots = new List<VoteRoot>();
            foreach (var uri in relatedVoteUris)
            {
                voteRoots.Add(await GetPropublicaVoteRecordAsync(uri));
            }

            var result = new List<string>();
            foreach (var item in voteRoots)
            {
                var memberPositions = item.Results.Votes.Vote.Positions
                    .SingleOrDefault(x => x.MemberId == member.Id);

                if (memberPositions is null)
                    continue;

                var chamber = member.Chamber == "House" ? "h" : "s";
                string billTitle = BillTitle(item, chamber);

                result.Add($"{member.Title} {member.FirstName} {member.LastName} " +
                    $"voted {memberPositions.VotePosition.ToUpper()} {billTitle}");
            }

            result.ForEach(x => Log.Debug(x));

            return result;
        }

        private string BillTitle(VoteRoot item, string chamber)
        {
            return $"{item.Results.Votes.Vote.Question} " +
                $"for {item.Results.Votes.Vote.Bill.BillId}: {item.Results.Votes.Vote.Bill.Title} " +
                $"{propublicaSettings.GovTrackUrl}/{chamber}{item.Results.Votes.Vote.RollCall}";
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