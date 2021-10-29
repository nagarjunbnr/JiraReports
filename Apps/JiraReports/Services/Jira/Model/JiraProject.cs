using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    public class JiraProject
    {
        public string Self { get; set; }
        public string ID { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string ProjectTypeKey { get; set; }
        public Dictionary<string, string> AvatarUrls { get; set; }

        public JiraProjectCategory ProjectCategory { get; set; }
    }
}
