using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    public class JiraFixVersion
    {
        public string Self { get; set; }
        public string ID { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public bool Archived { get; set; }
        public bool Released { get; set; }
    }
}
