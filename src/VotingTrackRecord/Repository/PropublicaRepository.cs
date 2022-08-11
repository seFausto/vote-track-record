using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Serilog;
using System.Security.Authentication;
using System.Text.Json;
using VotingTrackRecord.Common.PropublicaApiClasses;
using VotingTrackRecord.Common.Settings;
using VotingTrackRecordClasses;

namespace Repository
{
    public interface IPropublicaRepository
    {
        Task<Member?> GetMemberAsync(string userName);
        Task AddMemberAsync(string userName, string json);
        Task<VoteRoot?> GetVoteRecordAsync(string uri);
        Task AddVoteRecordAsync(string uri, string value);
        Task AddTweetAsync(long tweetId, string json);
        Task<bool> HasAlreadyBeenTweeted(long tweetId);
    }

    public class PropublicaRepository : IPropublicaRepository
    {
        private readonly DatabaseSettings databaseSettings;

        public PropublicaRepository(IOptions<DatabaseSettings> settings)
        {
            databaseSettings = settings.Value;
        }

        public async Task<Member?> GetMemberAsync(string userName)
        {
            try
            {
                var result = await GetMongoCollection(userName,
                    databaseSettings.MemberCollectionName, "UserName");

                if (result == null)
                    return null;

                var data = result.GetValue("Data");
                return JsonSerializer.Deserialize<Member>(data.AsString);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting member from database");
                throw;
            }
        }

        public async Task AddMemberAsync(string userName, string json)
        {
            BsonDocument bsonDocument = BsonDocument.Parse(
                new
                {
                    UserName = userName,
                    Data = json
                }.ToJson());

            await GetMongoDb()
                 .GetCollection<BsonDocument>(databaseSettings.MemberCollectionName)
                 .InsertOneAsync(bsonDocument);

        }

        public async Task<VoteRoot?> GetVoteRecordAsync(string uri)
        {
            try
            {
                var urlCollectionName = databaseSettings.UrlCollectionName;
                var docName = "Url";
                BsonDocument result = await GetMongoCollection(uri, urlCollectionName, docName);

                if (result == null)
                    return null;

                var data = result.GetValue("Data");
                return JsonSerializer.Deserialize<VoteRoot>(data.AsString);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting Vote root from database: {uri}", uri);
                throw;
            }
        }

        public async Task AddVoteRecordAsync(string uri, string json)
        {
            var bsonDocument = BsonDocument.Parse(
                new
                {
                    Url = uri,
                    Data = json
                }.ToJson());

            await GetMongoDb()
                 .GetCollection<BsonDocument>(databaseSettings.UrlCollectionName)
                 .InsertOneAsync(bsonDocument);

        }

        public async Task AddTweetAsync(long tweetId, string json)
        {
            var bsonDocument = BsonDocument.Parse(
                new
                {
                    TweetId = tweetId,
                    Data = json
                }.ToJson());

            await GetMongoDb()
                 .GetCollection<BsonDocument>(databaseSettings.TweetCollectionName)
                 .InsertOneAsync(bsonDocument);
        }

        public async Task<bool> HasAlreadyBeenTweeted(long tweetId)
        {
            try
            {
                return await GetMongoDb()
                    .GetCollection<BsonDocument>(databaseSettings.TweetCollectionName)
                    .Find(new BsonDocument("TweetId", tweetId))
                    .AnyAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting tweet from database: {TweetId}", tweetId);
                throw;
            }
        }

        private async Task<BsonDocument> GetMongoCollection(string dataId,
            string urlCollectionName, string docName)
        {
            return await GetMongoDb()
                .GetCollection<BsonDocument>(urlCollectionName)
                .Find(new BsonDocument(docName, dataId))
                .FirstOrDefaultAsync<BsonDocument>();
        }

        private IMongoDatabase GetMongoDb()
        {
            return GetMongoClient().GetDatabase(databaseSettings.DatabaseName);
        }

        private IMongoClient GetMongoClient()
        {
            var settings = MongoClientSettings.FromUrl(
                new MongoUrl(databaseSettings.ConnectionString));

            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            return new MongoClient(settings);
        }
    }
}
