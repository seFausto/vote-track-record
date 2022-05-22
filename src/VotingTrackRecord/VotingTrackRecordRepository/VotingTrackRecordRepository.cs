using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Security.Authentication;
using System.Text.Json;
using VotingTrackRecord.Common.Settings;

namespace DatabaseRepository
{
    public interface IVotingTrackRecordRepository
    {
        Task<bool> Add(string url, string body);
    }

    public class VotingTrackRecordRepository : IVotingTrackRecordRepository
    {
        private class PropublicaUrlResult
        {
            public string Url { get; set; }
            public string Body { get; set; }
        }

        private readonly DatabaseSettings databaseSettings;

        public VotingTrackRecordRepository(IOptions<DatabaseSettings> databaseSettings)
        {
            this.databaseSettings = databaseSettings.Value;
        }


        public async Task<bool> Add(string url, string body)
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(
              new MongoUrl(databaseSettings.ConnectionString)
            );

            settings.SslSettings =
              new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            var mongoClient = new MongoClient(settings);

            var result = mongoClient.GetDatabase(databaseSettings.DatabaseName)
                        .GetCollection<string>(databaseSettings.CollectionName);


            var json = JsonSerializer.Serialize(new PropublicaUrlResult() { Url = url, Body = body });
            
            await result.InsertOneAsync(json);

            return true;
        }
    }
}