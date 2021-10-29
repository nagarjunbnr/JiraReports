using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira.Model
{
    public class AgileBoard
    {
        public string ID { get; set; }
        public string Self { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
