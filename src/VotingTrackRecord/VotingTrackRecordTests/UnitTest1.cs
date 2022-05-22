using VoteTracker;

namespace VotingTrackRecordTests
{
    [TestClass]
    public class VoteTrackerBusinessTests
    {
        [TestMethod]
        public void QueryPropublicaWithNameAndKeyword()
        {
            var voteTrackerBusiness = new VoteTrackerBusiness();
            var result = voteTrackerBusiness.GetVotesHistoryAsync("Rep. John Smith",  new List<string>{ "veteran" });
        }
    }
}