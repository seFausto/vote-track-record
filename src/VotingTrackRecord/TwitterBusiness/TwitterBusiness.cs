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
        public DateTimeOffset LastTweet { get; set; }
        public DateTimeOffset LastCheck { get; set; }
    }


    public class TwitterBusiness : ITwitterBusiness
    {
        private const int BatchSize = 5;

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
                    LastTweet = DateTimeOffset.Now.AddDays(-1),
                    LastCheck = DateTimeOffset.MinValue
                });
            }
        }

        public async Task GetTweetsAsync()
        {
            if (friendIdsLastTweet is null)
                return;

            var userClient = new TwitterClient(twitterSettings.ApiKey, twitterSettings.ApiKeySecret,
                twitterSettings.AccessToken, twitterSettings.AccessTokenSecret);

            foreach (var item in friendIdsLastTweet.OrderBy(x => x.LastCheck).Take(BatchSize))
            {
                item.LastCheck = DateTimeOffset.Now;

                var tweets = await userClient.Timelines.GetUserTimelineAsync(item.FriendId);

                foreach (var tweet in tweets
                                        .Where(x => x.CreatedAt > item.LastTweet)
                                        .OrderBy(x => x.CreatedAt))
                {
                    if (tweet is null)
                        continue;

                    if (await propublicaRepository.HasAlreadyBeenTweeted(tweet.Id))
                        continue;

                    if (tweet.CreatedAt > item.LastTweet)
                    {
                        item.LastTweet = tweet.CreatedAt;
                        
                        Log.Debug("Tweet from {ScreenName}: {TweetText}", 
                            tweet.CreatedBy.ScreenName, tweet.FullText);

                        var member = await voteTrackerBusiness.GetPropublicaMemberInformationAsync(
                                tweet.CreatedBy.ScreenName, tweet.CreatedBy.Name);
                        
                        if (member == null)
                        {
                            Log.Error("Propublica member not found: {UserName}, {Name}", 
                                tweet.CreatedBy.ScreenName, tweet.CreatedBy.Name);
                            
                            return;
                        }
                        
                        var messages = await voteTrackerBusiness.GetReplyMessage(tweet.CreatedBy.ScreenName,
                            tweet.CreatedBy.Name, tweet.FullText, member);

                        if (messages.HasItems())
                        {
                            await ReplyToUsersTweetAsync(tweet, messages);
                        }

                        await propublicaRepository.AddTweetAsync(tweet.Id, JsonSerializer.Serialize(messages));
                    }
                }
            }
        }

        public async Task ReplyToUsersTweetAsync(Tweetinvi.Models.ITweet tweet, IEnumerable<string> messages)
        {
            var userClient = new TwitterClient(twitterSettings.ApiKey, twitterSettings.ApiKeySecret,
                twitterSettings.AccessToken, twitterSettings.AccessTokenSecret);

            foreach (var item in messages)
            {
                var message = $"@{tweet.CreatedBy} {item}";

                try
                {
                    _ = await userClient.Tweets.PublishTweetAsync(new PublishTweetParameters(message)
                    {
                        InReplyToTweet = tweet
                    });
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error replying to tweet");
                    throw;
                }

            }
        }
    }
}