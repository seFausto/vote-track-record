using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingTrackRecord.Common.Settings
{
    public class ApplicationSettings
    {
        public string LoggingHttpEndpoint { get; set; } = string.Empty;
        public string XApiKey { get; set; } = string.Empty;
        public int BatchSize { get; set; } = 2;
    }
}
