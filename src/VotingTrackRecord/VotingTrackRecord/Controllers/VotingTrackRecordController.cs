using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using TwitterService;
using VoteTracker;
using VotingTrackRecord.Common.Settings;

namespace VotingTrackRecord.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotingTrackRecordController : ControllerBase
    {
        private readonly ApplicationSettings ApplicationSettings;
        private readonly ITwitterBusiness twitterBusiness;

        public VotingTrackRecordController(ITwitterBusiness twitterBusiness,
            IOptions<ApplicationSettings> appSettings)
        {
            this.ApplicationSettings = appSettings.Value;
            this.twitterBusiness = twitterBusiness;
        }

        [HttpGet]
        public IActionResult Check()
        {
            return new OkResult();
        }

        [HttpGet("run")]
        public async Task<IActionResult> RunProcess([FromQuery] string apiKey)
        {
            try
            {
                if (!IsApiKeyValid(apiKey))
                    return new BadRequestResult();

                await twitterBusiness.GetTweetsAsync();
                return new OkResult();
            }
            catch (Exception)
            {
                throw;
            }

        }

        private bool IsApiKeyValid(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Log.Error("IP {IPAddress} called with missing apiKey",
                    Request.HttpContext.Connection.RemoteIpAddress);
                return false;
            }


            if (!ApplicationSettings.XApiKey.Equals(apiKey))
            {
                Log.Error("IP {IPAddress} called with invalid apiKey",
                    Request.HttpContext.Connection.RemoteIpAddress);

                return false;
            }

            return true;
        }
    }
}
