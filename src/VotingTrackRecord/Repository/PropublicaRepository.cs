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
        Task AddMemberAsync(string userName, string json);
        Task<Member?> GetMemberAsync(string userName);
        Task<Vote?> GetVoteRecordAsync(string uri);
        Task AddVoteRecordAsync(string uri, object value);
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

            MongoClientSettings settings = MongoClientSettings.FromUrl(
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

        public Task<Vote?> GetVoteRecordAsync(string uri)
        {
            throw new NotImplementedException();
        }

        public Task AddVoteRecordAsync(string uri, object value)
        {
            throw new NotImplementedException();
        }
    }
}