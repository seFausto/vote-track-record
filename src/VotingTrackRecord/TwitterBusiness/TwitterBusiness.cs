using Microsoft.Extensions.Options;
using Tweetinvi;
using VotingTrackRecord.Common.Settings;

namespace TwitterService
{
    public interface ITwitterBusiness
    {
        Task<string> GetTweets();
    }

    public class TwitterBusiness : ITwitterBusiness
    {
        private readonly TwitterSettings twitterSettings;
        public TwitterBusiness(IOptions<TwitterSettings> options)
        {
            this.twitterSettings = options.Value;
        }

        public async Task<string> GetTweets()
        {
            var userClient = new TwitterClient(twitterSettings.ApiKey,
                twitterSettings.ApiKeySecret, twitterSettings.AccessToken, twitterSettings.AccessTokenSecret);

            var friendsId = await userClient.Users.GetFriendIdsAsync(twitterSettings.UserId);

            var result = string.Empty;
            foreach (var item in friendsId)
            {
                result += item.ToString();
            }

            Console.WriteLine(result);
            return result;
        }
    }
}