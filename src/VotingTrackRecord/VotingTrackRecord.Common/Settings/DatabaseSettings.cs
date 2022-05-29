using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingTrackRecord.Common.Settings
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string UrlCollectionName { get; set; } = string.Empty;
        public string MemberCollectionName { get; set; } = string.Empty;
    }
}
