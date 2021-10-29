using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.KeyPoints.Productivity
{
    public class ProductivityTeamTotals
    {

        public string Project { get; set; }

        public int Sprint { get; set; }

        public decimal Productivity
        {
            get
            {
                return ProjectedHours > 0 ? Math.Round((LoggedHours / ProjectedHours) * 100, 2) : 0m;
            }
        }

        [DisplayName("Projected")]
        public decimal ProjectedHours { get; set; }

        [DisplayName("Hours")]
        public decimal LoggedHours { get; set; }

    }
}
