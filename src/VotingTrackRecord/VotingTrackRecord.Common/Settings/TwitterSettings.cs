using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingTrackRecord.Common.Settings
{
    public class TwitterSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ApiKeySecret{ get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string AccessTokenSecret { get; set; } = string.Empty;

        public long UserId { get; set; }

    }
}
