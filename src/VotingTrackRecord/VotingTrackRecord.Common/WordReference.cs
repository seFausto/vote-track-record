#nullable disable

using System.Text.Json.Serialization;

namespace Repository
{
    public class WordList
    {
        [JsonPropertyName("wordList")]
        public List<WordReference> WordReferences { get; set; } = new List<WordReference>();
    }
    public class WordReference
    {
        [JsonPropertyName("word")]
        public string Word { get; set; }
        [JsonPropertyName("related")]
        public IEnumerable<string> Related { get; set; }
    }
}