using Microsoft.AspNetCore.Mvc;
using VotingTrackRecord.Common.TwitterApiClasses;
using Serilog;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using VotingTrackRecord.Common.Settings;

namespace VotingTrackRecord.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwitterWebhookReceiverController : ControllerBase
    {
        private readonly TwitterSettings twitterSettings;
        public TwitterWebhookReceiverController(IOptions<TwitterSettings> settings)
        {
            this.twitterSettings = settings.Value;
        }

        [HttpGet("{challange}")]
        public string WebhookChallange(string challenge)
        {
            Log.Information("Webhook challange received {Challange}", challenge);

            var key = twitterSettings.ConsumerSecret;

            using var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            
            var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(challenge));
            
            return JsonSerializer.Serialize(
                new
                {
                    response_token = $"sha256={Convert.ToBase64String(hash)}"
                });
        }

        [HttpPost]
        public void Post([FromBody] TweetCreateEvent value)
        {
            Log.Information("Received a tweet from {0}", value);
        }
    }
}
