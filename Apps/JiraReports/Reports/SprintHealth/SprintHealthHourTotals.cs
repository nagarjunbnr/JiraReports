using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.SprintHealth
{
    public class SprintHealthHourTotals
    {
        public decimal ProjectedHours
        {
            get
            {
                return this.ProjectedEngHours + this.ProjectedQAHours;
            }
        }

        public decimal PlannedHours { get; set; }

        public decimal LoggedHours
        {
            get; set;
        }

        public decimal EngFillRate
        {
            get
            {
                return this.ProjectedEngHours > 0 ? Math.Round((this.PlannedEngHours / this.ProjectedEngHours) * 100, 2) : 0m;
            }
        }

        public decimal EngNonInquiryFillRate
        {
            get
            {
                return this.ProjectedEngHours > 0 ? Math.Round((this.PlannedNonInquiryEngHours / this.ProjectedEngHours) * 100, 2) : 0m;
            }
        }

        public decimal ProjectedEngHours { get; set; }

        public decimal PlannedEngHours { get; set; }

        public decimal PlannedNonInquiryEngHours { get; set; }

        public decimal LoggedEngHours { get; set; }

        public decimal QAFillRate
        {
            get
            {
                return this.ProjectedQAHours > 0 ? Math.Round((this.PlannedQAHours / ProjectedQAHours) * 100, 2) : 0m;
            }
        }

        public decimal ProjectedQAHours { get; set; }

        public decimal PlannedQAHours { get; set; }

        public decimal LoggedQAHours { get; set; }

    }
}
