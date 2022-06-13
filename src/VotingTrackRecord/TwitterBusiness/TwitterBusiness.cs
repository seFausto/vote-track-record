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
        Task GetTweetsAsync();
    }

    internal class FriendIdTweetTime
    {
        public long FriendId { get; set; }
        public long LastTweetId { get; set; }
        public DateTimeOffset LastCheck { get; set; }
    }


    public class TwitterBusiness : ITwitterBusiness
    {
        private const int BatchSize = 1;

        private readonly TwitterSettings twitterSettings;

        private readonly IPropublicaBusiness voteTrackerBusiness;

        private readonly IPropublicaRepository propublicaRepository;

        private static List<FriendIdTweetTime>? friendIdsLastTweet = null;

        public TwitterBusiness(IOptions<TwitterSettings> options, IPropublicaBusiness voteTrackerBusiness,
            IPropublicaRepository propublicaRepository)
        {
            this.twitterSettings = options.Value;

            this.voteTrackerBusiness = voteTrackerBusiness;

            this.propublicaRepository = propublicaRepository;

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
                friendIdsLastTweet.Add(new FriendIdTweetTime
                {
                    FriendId = friendId,
                    LastCheck = DateTimeOffset.MinValue,
                });
            }
        }

        public async Task GetTweetsAsync()
        {
            if (friendIdsLastTweet is null)
                return;
            
            var userClient = GetTwitterClient();

            foreach (var item in friendIdsLastTweet.OrderBy(x => x.LastCheck).Take(BatchSize))
            {
                item.LastCheck = DateTimeOffset.Now;
                
                var tweets = await userClient.Timelines.GetUserTimelineAsync(item.FriendId);
                                
                Log.Debug("Returned {Count} tweets for {FriendId}", tweets.Length, item.FriendId);
                
                foreach (var tweet in tweets.Where(x => x.Id > item.LastTweetId)
                                            .OrderByDescending(x => x.Id))
                {
                    if (tweet is null)
                        continue;

                    if (tweet.Id < item.LastTweetId)
                        continue;

                    item.LastTweetId = tweet.Id;

                    if (await propublicaRepository.HasAlreadyBeenTweeted(tweet.Id))
                        continue;

                    Log.Debug("Tweet from {ScreenName}: {TweetText}",
                        tweet.CreatedBy.ScreenName, tweet.FullText);

                    var member = await voteTrackerBusiness.GetPropublicaMemberInformationAsync(
                            tweet.CreatedBy.ScreenName, tweet.CreatedBy.Name);

                    if (member is null)
                    {
                        Log.Error("Propublica member not found: {UserName}, {Name}",
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
        }

        private TwitterClient GetTwitterClient()
        {
            return new TwitterClient(twitterSettings.ApiKey, twitterSettings.ApiKeySecret,
                twitterSettings.AccessToken, twitterSettings.AccessTokenSecret);
        }

        public async Task QuoteTweetWithVoteData(Tweetinvi.Models.ITweet tweet, IEnumerable<string> messages)
        {
            var userClient = GetTwitterClient();

            foreach (var item in messages)
            {
                var message = $"{item}";
                try
                {
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