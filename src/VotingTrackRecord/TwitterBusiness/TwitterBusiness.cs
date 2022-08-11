using Extensions;
using Microsoft.Extensions.Options;
using Repository;
using Serilog;
using System.Text.Json;
using Tweetinvi;
using Tweetinvi.Parameters;
using VoteTracker;
using VotingTrackRecord.Common.Settings;

namespace TwitterService
{
    public interface ITwitterBusiness
    {
        Task ProcessTweetsAsync();
    }

    internal class FriendIdTweetTime
    {
        public long FriendId { get; set; }
        public long LastTweetId { get; set; }
        public DateTimeOffset LastCheck { get; set; }
    }


    public class TwitterBusiness : ITwitterBusiness
    {
        private readonly int BatchSize;

        private readonly TwitterSettings twitterSettings;

        private readonly IPropublicaBusiness voteTrackerBusiness;

        private readonly IPropublicaRepository propublicaRepository;

        private static List<FriendIdTweetTime>? friendIdsLastTweet = null;

        public TwitterBusiness(IOptions<TwitterSettings> options, IOptions<ApplicationSettings> appSettings,
            IPropublicaBusiness voteTrackerBusiness, IPropublicaRepository propublicaRepository)
        {
            this.twitterSettings = options.Value;
                
            this.voteTrackerBusiness = voteTrackerBusiness;

            this.propublicaRepository = propublicaRepository;

            BatchSize = appSettings.Value.BatchSize;

            Task.Run(() => SetFriendIdsAsync()).Wait();
        }

        private async Task SetFriendIdsAsync()
        {
            if (friendIdsLastTweet is not null)
                return;

            friendIdsLastTweet = new List<FriendIdTweetTime>();

            var userClient = new TwitterClient(twitterSettings.ApiKey,
                twitterSettings.ApiKeySecret, twitterSettings.AccessToken, twitterSettings.AccessTokenSecret);

            var friendsIds = await userClient.Users.GetFriendIdsAsync(twitterSettings.UserId);

            foreach (var friendId in friendsIds)
            {
                Log.Information("Adding {FriendId} to list", friendId);

                friendIdsLastTweet.Add(new FriendIdTweetTime
                {
                    FriendId = friendId,
                    LastCheck = DateTimeOffset.MinValue,
                });
            }
        }

        public async Task ProcessTweetsAsync()
        {
            if (friendIdsLastTweet is null)
                return;

            var userClient = GetTwitterClient();

            foreach (var item in friendIdsLastTweet.OrderBy(x => x.LastCheck).Take(BatchSize))
            {
                item.LastCheck = DateTimeOffset.Now;

                var tweets = await userClient.Timelines.GetUserTimelineAsync(item.FriendId);

                Log.Information("Returned {Count} tweets for {FriendId}", tweets.Length, item.FriendId);

                var tweet = tweets.MaxBy(x => x.Id);

                if (tweet is null || tweet.Id < item.LastTweetId)
                    continue;

                item.LastTweetId = tweet.Id;

                if (await propublicaRepository.HasAlreadyBeenTweeted(tweet.Id))
                {
                    Log.Information("{ScreenName} Tweet has already been analyzed {TweetId}",
                        tweet.CreatedBy.ScreenName, tweet.Id);
                    continue;
                }

                Log.Information("Tweet from {ScreenName}: {TweetText}",
                    tweet.CreatedBy.ScreenName, tweet.FullText);

                var member = await voteTrackerBusiness.GetPropublicaMemberInformationAsync(
                        tweet.CreatedBy.ScreenName, tweet.CreatedBy.Name);

                if (member is null)
                {
                    Log.Error("Propublica member not found: {ScreenName}, {Name}",
                        tweet.CreatedBy.ScreenName, tweet.CreatedBy.Name);

                    return;
                }

                var messages = await voteTrackerBusiness.GetReplyMessage(tweet.CreatedBy.ScreenName,
                    tweet.CreatedBy.Name, tweet.FullText, member);

                if (messages.HasItems())
                {
                    await QuoteTweetWithVoteData(tweet, messages);
                }

                await propublicaRepository.AddTweetAsync(tweet.Id, JsonSerializer.Serialize(messages));
            }
        }

        private TwitterClient GetTwitterClient()
        {
            return new TwitterClient(twitterSettings.ApiKey, twitterSettings.ApiKeySecret,
                twitterSettings.AccessToken, twitterSettings.AccessTokenSecret);
        }

        public async Task QuoteTweetWithVoteData(Tweetinvi.Models.ITweet tweet, IEnumerable<string> messages)
        {
            var userClient = GetTwitterClient();

            foreach (var message in messages)
            {
                try
                {
                    Log.Information("Quoting Tweet {TweetId} from {ScreenName}: {Message}", tweet.Id,
                        tweet.CreatedBy.ScreenName, message);

                    _ = await userClient.Tweets.PublishTweetAsync(new PublishTweetParameters(message)
                    {
                        QuotedTweet = tweet
                    });
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error quoting tweet {TweetId}", tweet.Id);
                    throw;
                }

            }
        }
    }
}