using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoteTracker;

namespace VotingTrackRecord.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotingTrackRecordController : ControllerBase
    {
        private readonly IPropublicaBusiness voteTrackerBusiness;
        public VotingTrackRecordController(IPropublicaBusiness voteTrackerBusiness)
        {
            this.voteTrackerBusiness = voteTrackerBusiness;
        }
    }
}
