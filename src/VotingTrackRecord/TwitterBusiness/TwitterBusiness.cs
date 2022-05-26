using Microsoft.Extensions.Options;
using Tweetinvi;
using VoteTracker;
using VotingTrackRecord.Common.Settings;

namespace TwitterService
{
    public interface ITwitterBusiness
    {
        Task GetTweets();
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

        private readonly IVoteTrackerBusiness voteTrackerBusiness;

        private static List<FriendIdTweetTime>? friendIdsLastTweet = null;


        public TwitterBusiness(IOptions<TwitterSettings> options, IVoteTrackerBusiness voteTrackerBusiness)
        {
            this.twitterSettings = options.Value;

            this.voteTrackerBusiness = voteTrackerBusiness;

            SetFriendIds();
        }

        private void SetFriendIds()
        {
            if (friendIdsLastTweet is not null)
                return;

            friendIdsLastTweet = new List<FriendIdTweetTime>();

            var userClient = new TwitterClient(twitterSettings.ApiKey,
                twitterSettings.ApiKeySecret, twitterSettings.AccessToken, twitterSettings.AccessTokenSecret);

            var friendsIds = userClient.Users.GetFriendIdsAsync(twitterSettings.UserId).Result;

            foreach (var friendId in friendsIds)
            {
                friendIdsLastTweet.Add(new FriendIdTweetTime
                {
                    FriendId = friendId,
                    LastTweet = DateTimeOffset.MinValue,
                    LastCheck = DateTimeOffset.MinValue
                });
            }
        }

        public async Task GetTweets()
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
                    await voteTrackerBusiness.ProcessTweet(latestTweet.FullText, latestTweet.CreatedBy?.ToString() ?? string.Empty);

                    Console.WriteLine($"{latestTweet.CreatedBy} -  {latestTweet.Text}");
                }
            }
        }
    }
}