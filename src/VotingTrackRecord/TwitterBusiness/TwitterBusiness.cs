using Microsoft.Extensions.Options;
using Tweetinvi;
using VotingTrackRecord.Common.Settings;

namespace TwitterService
{
    public interface ITwitterBusiness
    {
        Task GetTweets();
    }

    public class TwitterBusiness : ITwitterBusiness
    {
        private readonly TwitterSettings twitterSettings;
        private static Dictionary<long, DateTimeOffset> friendIdsLastTweet = null;
        public TwitterBusiness(IOptions<TwitterSettings> options)
        {
            this.twitterSettings = options.Value;
            SetFriendIds();          
        }

        private  void SetFriendIds()
        {
            if (friendIdsLastTweet is not null)
                return;
            
            friendIdsLastTweet = new Dictionary<long, DateTimeOffset>();
            
            var userClient = new TwitterClient(twitterSettings.ApiKey,
                twitterSettings.ApiKeySecret, twitterSettings.AccessToken, twitterSettings.AccessTokenSecret);

            var friendsIds =  userClient.Users.GetFriendIdsAsync(twitterSettings.UserId).Result;

            foreach (var friendId in friendsIds)
            {
                friendIdsLastTweet.Add(friendId, DateTimeOffset.MinValue);
            }
        }

        public async Task GetTweets()
        {
            var userClient = new TwitterClient(twitterSettings.ApiKey,
                twitterSettings.ApiKeySecret, twitterSettings.AccessToken, twitterSettings.AccessTokenSecret);
            
            foreach (var item in friendIdsLastTweet)
            {
                var tweets = await userClient.Timelines.GetUserTimelineAsync(item.Key);

                var latestTweet = tweets?.FirstOrDefault();
                
                 if (latestTweet is null)
                    continue;

                if (latestTweet.CreatedAt > friendIdsLastTweet[item.Key])
                {
                    friendIdsLastTweet[item.Key] = latestTweet.CreatedAt;
                    Console.WriteLine($"{latestTweet.CreatedBy} -  {latestTweet.Text}");
                }
            }
        }
    }
}