using JiraReports.Services;
using JiraReports.Services.Jira;
using JiraReports.Services.Jira.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.Common.EffectiveValue
{
    public class SprintStatsIssue
    {

        public SprintStatsIssue()
        {
            this.PredictabilityStatus = "N/A";
        }

        public JiraIssue Issue { get; set; }

        public List<JiraWorklog> Worklog { get; set; }

        public string IssueType
        {
            get
            {
                return this.Issue.Fields.IssueType?.Name;
            }
        }
        //Added by Nirav Desai
        public string Components { get; set; }
        public string PredictabilityStatus { get; set; }

        public string IncompleteReason { get; set; }

        public bool IsValueAdded { get; set; }

        public int DefectCount { get; set; }

        public decimal LoggedHours
        {
            get
            {
                return this.Worklog.Sum(d => d.Hours);
            }
        }

        public string Summary { get; set; }

        public string Resolution
        {
            get
            {
                return this.Issue.Fields.Resolution?.Name;
            }
        }

        public decimal EstimatedHours { get; set; }

        public string[] GetEngineeringContributors(IPersonnelService personnelService, string project, string sprintNumber)
        {
            return GetContributors(personnelService, project, sprintNumber, PersonRole.Eng);
        }

        public string[] GetQAContributors(IPersonnelService personnelService, string project, string sprintNumber)
        {
            return GetContributors(personnelService, project, sprintNumber, PersonRole.QA);
        }

        public decimal GetLoggedEngHours(IPersonnelService personnelService, string project, string sprintNumber)
        {
           decimal result = 0;
           string[] eng = this.GetEngineeringContributors(personnelService, project, sprintNumber);

           foreach (var w in this.Worklog)
           {
                if (eng.Contains(w.Author.DisplayName))
                {
                    result += w.Hours;
                }
           }

           return result;           
        }

        public decimal GetLoggedEngDevHours(IPersonnelService personnelService, string project, string sprintNumber, DateTime startDate, DateTime endDate)
        {
            decimal result = 0;
            string[] eng = this.GetEngineeringContributors(personnelService, project, sprintNumber);
            startDate = startDate.Date;
            endDate = endDate.Date;
            foreach (var w in this.Worklog)
            {
                var started = w.Started.Date;
                

                 if (startDate < started && started < endDate.AddDays(-1))
                {
                    if (eng.Contains(w.Author.DisplayName))
                    {
                        result += w.Hours;
                    }
                }

                
            }

            return result;
        }

        public decimal GetLoggedEngDefectFixHours(IPersonnelService personnelService, string project, string sprintNumber, DateTime startDate, DateTime endDate)
        {
            decimal result = 0;
            string[] eng = this.GetEngineeringContributors(personnelService, project, sprintNumber);

            startDate = startDate.Date;
            endDate = endDate.Date;
            foreach (var w in this.Worklog)
            {
                var started = w.Started.Date;
                 if (started >= endDate.AddDays(-1))
                {
                    if (eng.Contains(w.Author.DisplayName))
                    {
                        result += w.Hours;
                    }
                }
                
            }

            return result;
        }

        public decimal GetLoggedEngPlanHours(IPersonnelService personnelService, string project, string sprintNumber, DateTime startDate, DateTime endDate)
        {
            decimal result = 0;
            string[] eng = this.GetEngineeringContributors(personnelService, project, sprintNumber);

            startDate = startDate.Date;
            endDate = endDate.Date;
            foreach (var w in this.Worklog)
            {
                var started = w.Started.Date;
                if (started.Equals(startDate))
                {
                    if (eng.Contains(w.Author.DisplayName))
                    {
                        result += w.Hours;
                    }
                }

                
            }

            return result;
        }

        public decimal GetLoggedQATestHours(IPersonnelService personnelService, string project, string sprintNumber, DateTime startDate, DateTime endDate)
        {
            decimal result = 0;
            string[] eng = this.GetQAContributors(personnelService, project, sprintNumber);
            startDate = startDate.Date;
            endDate = endDate.Date;
            foreach (var w in this.Worklog)
            {
                var started = w.Started.Date;


                if (startDate < started && started < endDate.AddDays(-1))
                {
                    if (eng.Contains(w.Author.DisplayName))
                    {
                        result += w.Hours;
                    }
                }


            }

            return result;
        }


        public decimal GetLoggedQARegressionHours(IPersonnelService personnelService, string project, string sprintNumber, DateTime startDate, DateTime endDate)
        {
            decimal result = 0;
            string[] eng = this.GetQAContributors(personnelService, project, sprintNumber);
            startDate = startDate.Date;
            endDate = endDate.Date;
            foreach (var w in this.Worklog)
            {
                var started = w.Started.Date;
                if (started >= endDate.AddDays(-1))
                {
                    if (eng.Contains(w.Author.DisplayName))
                    {
                        result += w.Hours;
                    }
                }

            }
            return result;
        }

        public decimal GetLoggedQAPlanHours(IPersonnelService personnelService, string project, string sprintNumber, DateTime startDate, DateTime endDate)
        {
            decimal result = 0;
            string[] eng = this.GetQAContributors(personnelService, project, sprintNumber);
            startDate = startDate.Date;
            endDate = endDate.Date;
            foreach (var w in this.Worklog)
            {
                var started = w.Started.Date;
                if (started.Equals(startDate))
                {
                    if (eng.Contains(w.Author.DisplayName))
                    {
                        result += w.Hours;
                    }
                }


            }
            return result;
        }
        public decimal GetLoggedQAHours(IPersonnelService personnelService, string project, string sprintNumber)
        {
            decimal result = 0;
            string[] eng = this.GetQAContributors(personnelService, project, sprintNumber);

            foreach (var w in this.Worklog)
            {
                if (eng.Contains(w.Author.DisplayName))
                {
                    result += w.Hours;
                }
            }

            return result;
        }


        private string[] GetContributors(IPersonnelService personnelService, string project, 
            string sprintNumber, PersonRole targetRole)
        {
            var result = new List<string>();
            var distinctContributors = this.Worklog.Select(w => w.Author.DisplayName).Distinct();

            foreach (string contributor in distinctContributors)
            {
                if (personnelService.GetFirstRole(contributor, project, sprintNumber) == targetRole)
                {
                    result.Add(contributor);
                }
            }

            return result.ToArray();
        }


    }
}
