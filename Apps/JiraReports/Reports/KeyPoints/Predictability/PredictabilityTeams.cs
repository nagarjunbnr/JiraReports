using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.KeyPoints.Predictability
{
    public class PredictabilityTeams
    {

        [DisplayName("Project")]
        public string Project { get; set; }

        [DisplayName("Predictability")]
        public decimal PredictabilityScore { get; set; }

    }
}
