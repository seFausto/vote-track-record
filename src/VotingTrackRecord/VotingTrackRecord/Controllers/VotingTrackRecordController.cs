using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoteTracker;

namespace VotingTrackRecord.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotingTrackRecordController : ControllerBase
    {
        private readonly IVoteTrackerBusiness voteTrackerBusiness;
        public VotingTrackRecordController(IVoteTrackerBusiness voteTrackerBusiness)
        {
            this.voteTrackerBusiness = voteTrackerBusiness;
        }

        [HttpGet("GetVotes")]
        public async Task<ObjectResult> GetVotesForKeywords(string name, string tweetText)
        {
            var result = await voteTrackerBusiness.GetVotesHistoryAsync(name, new List<string> { tweetText });

            return Ok(result);
        }
    }
}
