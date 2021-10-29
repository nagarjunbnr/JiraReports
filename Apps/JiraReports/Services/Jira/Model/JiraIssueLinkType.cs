using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    public class JiraIssueLinkType
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Inward { get; set; }
        public string Outward { get; set; }
        public string Self { get; set; }
    }
}
