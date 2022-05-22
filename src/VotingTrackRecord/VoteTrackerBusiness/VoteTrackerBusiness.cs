using DatabaseRepository;
using System.Collections.Generic;
using VotingTrackRecordClasses;

namespace VoteTracker
{
    public interface IVoteTrackerBusiness
    {
        Task<VotesHistory> GetVotesHistoryAsync(string name, IEnumerable<string> keywords);
    }

    public class VoteTrackerBusiness : IVoteTrackerBusiness
    {
        private readonly IVotingTrackRecordRepository votingTrackRecordRepository;
        private readonly IPropublicaService propublicaService;
        
        public VoteTrackerBusiness(IPropublicaService propublicaService,
            IVotingTrackRecordRepository votingTrackRecordRepository)
        {
            this.votingTrackRecordRepository = votingTrackRecordRepository;
            this.propublicaService = propublicaService;
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