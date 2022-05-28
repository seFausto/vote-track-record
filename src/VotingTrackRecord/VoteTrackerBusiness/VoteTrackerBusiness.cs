using Repository;
using System.Collections.Generic;
using VotingTrackRecordClasses;
using System.Text.Json;
using VotingTrackRecord.Common.PropublicaApiClasses;

namespace VoteTracker
{
    public interface IPropublicaBusiness
    {
        Task<VotesHistory> GetVotesHistoryAsync(string name, IEnumerable<string> keywords);

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

            //var votesHistory = await GetVotesHistoryAsync(officialName, keywords);

            //await votingTrackRecordRepository.SaveVotesHistoryAsync(votesHistory);
        }

        private async Task<IEnumerable<string>> GetKeywords(string tweetText)
        {
            return new List<string> { "veteran", "gun" };
        }

        private async Task<Member> GetPropublicaMemberInformation(string userName, string name)
        {
            //check db first if nothing then call api, then save to db
            var member = await propublicaRepository.GetMemberAsync(userName);
            if (member is null)
            {
                member = await propublicaApiService.GetMemberByNameAsync(userName, name);

                await propublicaRepository.AddMemberAsync(userName, JsonSerializer.Serialize(member));
            }

            return member;
        }


        public async Task<VotesHistory> GetVotesHistoryAsync(string name, IEnumerable<string> keywords)
        {
            var results = await propublicaApiService.SeachBills(keywords.First());

            return null;
        }
    }

    public class VotesHistory
    {
    }
}