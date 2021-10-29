using JiraReports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.KeyPoints.Productivity
{
    public class ProjectSprintPersonTotals
    {
        public string Project { get; set; }
        public int Sprint { get; set; }
        public string Person { get; set; }
        public string Roles { get; set; }
        public decimal Productivity => (Projected > 0)
            ? Math.Round(((Hours / Projected) * 100), 2) : 0M;
        public decimal Projected { get; set; } = 0;
        public decimal Hours { get; set; } = 0;
    }
}
