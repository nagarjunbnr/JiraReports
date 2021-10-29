using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.KeyPoints.Productivity
{
    public class PredictabilityData
    {

        [DisplayName("Project")]
        public string Project { get; set; }

        [DisplayName("Sprint")]
        public int Sprint { get; set; }

        [DisplayName("SprintId")]
        public string SprintId { get; set; }

        [DisplayName("Sprint Start")]
        public string SprintStartDate { get; set; }

        [DisplayName("Predictability")]
        public decimal PredictabilityScore { get; set; }

        [DisplayName("Predictability Pct")]
        public decimal PredictabilityScorePct => this.PredictabilityScore / 100M;

        [DisplayName("Planned Hours")]
        public decimal HoursPlanned { get; set; }

        [DisplayName("Completed Hours")]
        public decimal HoursCompleted { get; set; }

        [DisplayName("Pulled Hours")]
        public decimal HoursPulled { get; set; }

    }
}
