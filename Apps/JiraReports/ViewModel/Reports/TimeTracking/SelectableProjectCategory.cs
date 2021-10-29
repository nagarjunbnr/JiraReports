using JiraReports.Services.Jira;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.ViewModel.Reports.TimeTracking
{
    public class SelectableProjectCategory
    {

        public bool IsSelected { get; set; }

        public JiraProjectCategory Category { get; set; }

    }
}
