#nullable disable

namespace Repository
{
    public class WordReference
    {
        public string Word { get; set; }
        public IEnumerable<string> Related { get; set; }
    }
}