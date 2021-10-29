using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    public class JiraUserBasic
    {
        public string Self { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public Dictionary<string, string> AvatarUrls { get; set; }
        public string DisplayName { get; set; }
        public bool Active { get; set; }
        public string TimeZone { get; set; }
    }
}
