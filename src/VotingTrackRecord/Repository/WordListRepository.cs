using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VotingTrackRecord.Common.Settings;

namespace Repository
{
    public interface IWordListRepository
    {
        Task<IEnumerable<WordReference>> GetWordReferences();
    }

    public class WordListRepository : IWordListRepository
    {
        private readonly DatabaseSettings databaseSettings;

        public WordListRepository(IOptions<DatabaseSettings> databaseSettings)
        {
            this.databaseSettings = databaseSettings.Value;
        }

        public async Task<IEnumerable<WordReference>> GetWordReferences()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("WordList.json"));

            if (string.IsNullOrEmpty(resourceName))
                return new List<WordReference>();

            using Stream? stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
                return new List<WordReference>();

            using StreamReader reader = new(stream);

            string result = await reader.ReadToEndAsync();

            var wordReference = JsonSerializer.Deserialize<List<WordReference>>(result);

            return wordReference ?? new List<WordReference>();


        }
    }
}
