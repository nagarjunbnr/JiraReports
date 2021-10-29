using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    public class JiraEpicFieldSummary
    {
        public string Expand { get; set; }
        public string ID { get; set; }
        public string Self { get; set; }
        public string Key { get; set; }
        public JiraEpicField Fields { get; set; }
    }
}
