using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TwitterService;
using VoteTracker;

namespace VotingTrackRecord.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotingTrackRecordController : ControllerBase
    {
        private readonly ITwitterBusiness twitterBusiness;
        public VotingTrackRecordController(ITwitterBusiness twitterBusiness)
        {
            this.twitterBusiness = twitterBusiness;
        }
        
        [HttpGet]
        public IActionResult Check()
        {
            return new OkResult();
        }

        [HttpGet("run")]
        public async Task<IActionResult> RunProcess()
        {
            await twitterBusiness.GetTweetsAsync();
            return new OkResult();
        }
    }
}
