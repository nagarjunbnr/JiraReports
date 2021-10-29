using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.Common.EffectiveValue
{
    public class TeamSprintStatsView
    {

        public string Team { get; set; }

        public int Sprint { get; set; }

        public decimal TotalLoggedHours { get; set; }

        public decimal ValueAddedLoggedHours { get; set; }

        public decimal NonValueLoggedHours { get; set; }

        public decimal FailureRateHours { get; set; }

        public decimal Predictability { get; set; }

        public decimal TotalResources { get; set; }

        public decimal SprintValue { get; set; }

    }
}
