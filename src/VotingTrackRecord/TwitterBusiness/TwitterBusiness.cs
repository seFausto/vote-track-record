using Microsoft.Extensions.Options;
using Serilog;
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

        private static List<FriendIdTweetTime>? friendIdsLastTweet = null;


        public TwitterBusiness(IOptions<TwitterSettings> options, IPropublicaBusiness voteTrackerBusiness)
        {
            this.twitterSettings = options.Value;

            this.voteTrackerBusiness = voteTrackerBusiness;

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

            foreach (var friendId in friendsIds.Take(1))
            {
                friendIdsLastTweet.Add(new FriendIdTweetTime
                {
                    FriendId = friendId,
                    LastTweet = DateTimeOffset.MinValue,
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

                var latestTweet = tweets?.FirstOrDefault();

                if (latestTweet is null)
                    continue;

                if (latestTweet.CreatedAt > item.LastTweet)
                {
                    item.LastTweet = latestTweet.CreatedAt;
                    Log.Debug("Tweet from {ScreenName}: {TweetText}", latestTweet.CreatedBy.ScreenName, latestTweet.FullText);
                    
                    var messages = await voteTrackerBusiness.GetReplyMessage(latestTweet.CreatedBy.ScreenName, latestTweet.CreatedBy.Name,
                        latestTweet.FullText);

                    Log.Information("Messages: {@Messages}", messages);

                    await ReplyToUsersTweetAsync(latestTweet, messages);
                }
            }
        }

        public async Task ReplyToUsersTweetAsync(Tweetinvi.Models.ITweet? tweet, IEnumerable<string> messages)
        {
            var userClient = new TwitterClient(twitterSettings.ApiKey, twitterSettings.ApiKeySecret,
                twitterSettings.AccessToken, twitterSettings.AccessTokenSecret);

            var message = string.Join("\n", messages);

            var reply = await userClient.Tweets.PublishTweetAsync(new PublishTweetParameters(message)
            {
                InReplyToTweet = tweet
            });

        }
    }
}