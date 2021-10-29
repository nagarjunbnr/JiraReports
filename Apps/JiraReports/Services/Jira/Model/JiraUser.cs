using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    public class JiraUser : JiraUserBasic
    {
        public ItemList<JiraGroup> Groups { get; set; }
        public ItemList<JiraApplicationRole> ApplicationRoles { get; set; }
        public string Expand { get; set; }
    }
}
