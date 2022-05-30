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
            MongoClientSettings settings = MongoClientSettings.FromUrl(
                  new MongoUrl(databaseSettings.ConnectionString));

            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            var mongoClient = new MongoClient(settings);

            try
            {
                var result = await mongoClient.GetDatabase(databaseSettings.DatabaseName)
                    .GetCollection<BsonDocument>(databaseSettings.MemberCollectionName)
                    .Find(new BsonDocument("UserName", userName))
                    .FirstOrDefaultAsync<BsonDocument>();

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

            var settings = MongoClientSettings.FromUrl(
              new MongoUrl(databaseSettings.ConnectionString));

            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            var mongoClient = new MongoClient(settings);

            BsonDocument bsonDocument = BsonDocument.Parse(
                new
                {
                    UserName = userName,
                    Data = json
                }.ToJson());

            await mongoClient.GetDatabase(databaseSettings.DatabaseName)
                 .GetCollection<BsonDocument>(databaseSettings.MemberCollectionName)
                 .InsertOneAsync(bsonDocument);

        }

        public async Task<VoteRoot?> GetVoteRecordAsync(string uri)
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(
                 new MongoUrl(databaseSettings.ConnectionString));

            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            var mongoClient = new MongoClient(settings);

            try
            {
                var result = await mongoClient.GetDatabase(databaseSettings.DatabaseName)
                    .GetCollection<BsonDocument>(databaseSettings.UrlCollectionName)
                    .Find(new BsonDocument("Url", uri))
                    .FirstOrDefaultAsync<BsonDocument>();

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
            var settings = MongoClientSettings.FromUrl(
                new MongoUrl(databaseSettings.ConnectionString));

            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            var mongoClient = new MongoClient(settings);

            BsonDocument bsonDocument = BsonDocument.Parse(
                new
                {
                    Url = uri,
                    Data = json
                }.ToJson());

            await mongoClient.GetDatabase(databaseSettings.DatabaseName)
                 .GetCollection<BsonDocument>(databaseSettings.UrlCollectionName)
                 .InsertOneAsync(bsonDocument);

        }

        public async Task AddTweetAsync(long tweetId, string json)
        {
            var settings = MongoClientSettings.FromUrl(
                   new MongoUrl(databaseSettings.ConnectionString));

            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            var mongoClient = new MongoClient(settings);

            BsonDocument bsonDocument = BsonDocument.Parse(
                new
                {
                    TweetId = tweetId,
                    Data = json
                }.ToJson());

            await mongoClient.GetDatabase(databaseSettings.DatabaseName)
                 .GetCollection<BsonDocument>(databaseSettings.TweetCollectionName)
                 .InsertOneAsync(bsonDocument);
        }

        public async Task<bool> HasAlreadyBeenTweeted(long tweetId)
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(
                new MongoUrl(databaseSettings.ConnectionString));

            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            var mongoClient = new MongoClient(settings);

            try
            {
                var result = await mongoClient.GetDatabase(databaseSettings.DatabaseName)
                    .GetCollection<BsonDocument>(databaseSettings.TweetCollectionName)
                    .Find(new BsonDocument("TweetId", tweetId))
                    .AnyAsync();

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting tweet from database: {TweetId}", tweetId);
                throw;
            }
        }
    }
}