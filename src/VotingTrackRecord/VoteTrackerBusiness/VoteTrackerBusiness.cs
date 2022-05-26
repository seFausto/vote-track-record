using System.Collections.Generic;
using VotingTrackRecordClasses;

namespace VoteTracker
{
    public interface IVoteTrackerBusiness
    {
        Task<VotesHistory> GetVotesHistoryAsync(string name, IEnumerable<string> keywords);

        Task ProcessTweet(string tweetText, string userName);
    }

    public class VoteTrackerBusiness : IVoteTrackerBusiness
    {        
        private readonly IPropublicaService propublicaService;

        public VoteTrackerBusiness(IPropublicaService propublicaService)
        {           
            this.propublicaService = propublicaService;
        }

        public async Task ProcessTweet(string tweetText, string userName)
        {
            var officialName = await GetOfficialName(userName);

            var keywords = await GetKeywords(tweetText);

            //var votesHistory = await GetVotesHistoryAsync(officialName, keywords);

            //await votingTrackRecordRepository.SaveVotesHistoryAsync(votesHistory);
        }

        private async Task<IEnumerable<string>> GetKeywords(string tweetText)
        {
            return new List<string> { "veteran", "gun" };
        }

        private async Task<string> GetOfficialName(string userName)            
        {
            var member = await propublicaService.GetMemberByName(userName);
            
            return "Joe Biden";
        }


        public async Task<VotesHistory> GetVotesHistoryAsync(string name, IEnumerable<string> keywords)
        {
            var results = await propublicaService.SeachBills(keywords.First());

            return null;
        }
    }

    public class VotesHistory
    {
    }
}