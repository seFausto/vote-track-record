namespace VotingTrackRecord.Common.Settings
{
    public class PropublicaSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Congress { get; set; } = string.Empty;
        public string GovTrackUrl { get; set; } = string.Empty;
        public int PageSize { get; set; }
    }
}