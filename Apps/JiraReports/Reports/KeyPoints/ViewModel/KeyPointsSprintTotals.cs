using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.KeyPoints.ViewModel
{
    public class KeyPointsSprintTotals
    {

        public string Team { get; set; }

        public int Sprint { get; set; }

        public decimal Productivity { get; set; }

        public decimal Predictability { get; set; }

        public decimal SprintValue { get; set; }

        public decimal PredictabilityPct
        {
            get
            {
                return this.Predictability / 100M;
            }
        }

        public decimal ProductivityPct
        {
            get
            {
                return this.Productivity / 100M;
            }
        }

    }
}
