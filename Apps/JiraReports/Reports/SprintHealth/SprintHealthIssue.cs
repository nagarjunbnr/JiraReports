using JiraReports.Services.Jira;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.SprintHealth
{
    public class SprintHealthIssue
    {
        public string IssueKey { get; set; }

        public string IssueType { get; set; }

        public string Sprint { get; set; }

        public string Components { get; set; }
        public string PredictabilityStatus { get; set; }

        public int DefectCount { get; set; }

        public string IncompleteReason { get; set; }

        public string Resolution { get; set; }

        public decimal EstimatedHours { get; set; }

        public string EpicLink { get; set; }

        public string EpicInvestmentCategory { get; set; }

        public decimal TotalLoggedHours
        {
            get; set;
        }

        //public decimal TotalPointValue { get; set; }

        public string IssueBucket { get; set; }

        public decimal LoggedEngPlanHours { get; set; }

        public decimal LoggedEngDevHours { get; set; }

        public decimal LoggedEngDefectFixHours { get; set; }

        public decimal LoggedEngHours { get; set; }

        public decimal LoggedQAPlanHours { get; set; }

        public decimal LoggedQATestHours { get; set; }

        public decimal LoggedQARegressionHours { get; set; }

        public decimal LoggedQAHours { get; set; }

        public decimal EngEstimate { get; set; }

        public decimal QAEstimate { get; set; }

        public decimal? EngEstRatio
        {
            get
            {
                return EngEstimate > 0 ? Math.Round(LoggedEngHours / EngEstimate, 2) : (decimal?)null;
            }
        }

        public decimal? QAEstRatio
        {
            get
            {
                return QAEstimate > 0 ? Math.Round(LoggedQAHours / QAEstimate, 2) : (decimal?)null;
            }
        }

        public decimal? TotalHoursInReview { get; set; }

        public decimal? TotalHoursInCodeMerge { get; set; }

        public string Eng { get; set; }

        public string QA { get; set; }

        public string IssueId { get; set; }
        public List<JiraIssueLink> IssueLinks { get; internal set; }
    }
}
