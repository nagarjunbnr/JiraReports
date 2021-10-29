using JiraReports.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira.Model
{
    public class JiraIssue
    {
        public string Expand { get; set; }
        public string ID { get; set; }
        public string Self { get; set; }
        public string Key { get; set; }

        public JiraIssueChangelog Changelog { get; set; }

        [JsonConverter(typeof(JiraFieldsConverter))]
        public JiraFields Fields { get; set; }

        public List<JiraIssueActor> Actors { get; set; }

        public List<string> EngineerNames
        {
            get
            {
                return this.Actors.Where(a => a.Role == PersonRole.Eng).Select(d => d.Name).ToList();
            }
        }

        public List<string> QANames
        {
            get
            {
                return this.Actors.Where(a => a.Role == PersonRole.QA).Select(d => d.Name).ToList();
            }
        }

        public decimal? OriginalEstimateHours
        {
            get
            {
                decimal? result = null;

                if (!string.IsNullOrEmpty(this.Fields.TimeOriginalEstimate))
                {
                    int originalEstimateSeconds = 0;
                    int.TryParse(this.Fields.TimeOriginalEstimate, out originalEstimateSeconds);

                    result = Math.Round(originalEstimateSeconds > 0 ? originalEstimateSeconds / 60m / 60 : 0, 2);
                }

                return result;
            }
        }

        public decimal EngineeringEstimate
        {
            get
            {
                return ParseEstimateHours(this.Fields.OriginalEngHours);
            }
        }

        public decimal QAEstimate
        {
            get
            {
                return ParseEstimateHours(this.Fields.OriginalQAHours);
            }
        }

        public JiraIssue()
        {
            this.Actors = new List<JiraIssueActor>();
        }

        private bool IsWorklogAnalyzed { get; set; }

        public PulledIssueInfo GetPulledInfo(AgileSprint sprint)
        {
            var result = new PulledIssueInfo();

            var histories = this.Changelog.Histories;


            //check if case is still in the sprint

            if (this.Fields.Sprint == sprint.Name)
            {
                //if the issue is resolved and Incomplete, we consider it Pulled
                if (!string.IsNullOrEmpty(this.Fields.ResolutionDate) && this.IsIncomplete)
                {
                    result.isPulled = true;
                    //result.PulledOn = DateTime.Parse(this.Fields.ResolutionDate);

                    return result;
                }

                //otherwise, it's still in the sprint, no matter if it was pulled before
                return result;
            }

            foreach (var h in histories)
            {
                var sprintItems = h.Items.Where(i => i.Field == "Sprint").ToList();

                foreach (var sprintItem in sprintItems)
                {
                    //any sprint item transition that where the From Sprint doesn't appear in the To Sprint
                    //is counted as a case that had to leave the sprint it was in
                    if (sprintItem.From != null && sprintItem.To != null
                        && sprintItem.From == sprint.ID && !sprintItem.To.Contains(sprintItem.From) 
                        && h.Created > sprint.StartDate)
                    {
                        result.isPulled = true;
                        result.PulledOn = h.Created;
                    }
                }
            }

            return result;
        }

        public bool IsCompleted
        {
            get
            {
                return this.Fields.Status.Name.Equals("done", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        public bool IsPending
        {
            get
            {
                return !IsCompleted;
            }
        }

        public bool IsCustomerReportedBug
        {
            get
            {
                return this.Fields.IssueType.Name == "Bug" && this.Fields.ZendeskIssueCount > 0;
            }
        }

        public bool IsPredictabilityEligible
        {
            get
            {
                return this.Fields.IssueType.Name == "Feature" || this.Fields.IssueType.Name == "Bug";
            }
        }

        public void AnalyzeWorklog(IJiraIssueService issueService, IPersonnelService personnelService, string project, string sprint)
        {
            if (this.Fields.Worklog != null && !this.IsWorklogAnalyzed)
            {
                List<JiraWorklog> worklogs = this.Fields.Worklog.Worklogs;

                if(worklogs.Count == JiraConstants.MaxWorklogsPerIssue) // this is the max count of worklogs from standard API call
                {
                    worklogs = issueService.GetWorklogsForIssue(this.Key).Worklogs;
                }

                foreach (var w in worklogs)
                {
                    PersonRole role = personnelService.GetFirstRole(w.Author.DisplayName, project, sprint);
                    JiraIssueActor actor = this.Actors.FirstOrDefault(a => a.Name == w.Author.DisplayName);

                    if (actor == null)
                    {
                        actor = new JiraIssueActor()
                        {
                            Name = w.Author.DisplayName,
                            Role = role
                        };

                        this.Actors.Add(actor);
                    }

                    actor.TimeSpentSeconds += w.TimeSpentSeconds;
                }

                this.IsWorklogAnalyzed = true;
            }
        }

        public decimal? GetTotalHoursInReview()
        {
            return GetTotalHoursSpentInStatus("In Review");
        }

        public decimal? GetTotalHoursInCodeMerge()
        {
            return GetTotalHoursSpentInStatus("Code Merge");
        }

        public bool IsIncomplete
        {
            get
            {
                return !string.IsNullOrEmpty(this.Fields.IncompleteReason?.Value);
            }
        }

        public string GetIncompleteReason(AgileSprint targetSprint)
        {
            var pulledInfo = this.GetPulledInfo(targetSprint);

            return this.GetIncompleteReason(targetSprint, pulledInfo.PulledOn);
        }

        public string GetIncompleteReason(AgileSprint targetSprint, DateTime? sprintChangeDate = null)
        {
            if (sprintChangeDate != null)
            {
                return GetIncompleteReasonAtSprintChange(sprintChangeDate.Value);
            }
            else if (this.Fields.ResolutionDate != null)
            {
                return this.Fields.IncompleteReason?.Value;
            }

            return null;

            //return GetIncompleteReasonAtSprintChange(sprintChangeDate.Value);
        }

        public bool IsValidIncompleteReason(AgileSprint targetSprint)
        {
            string reason = GetIncompleteReason(targetSprint);

            return IsValidIncompleteReason(reason);
        }

        public bool IsValidIncompleteReason(string reason)
        {

            if (string.IsNullOrEmpty(reason))
            {
                return true; //Unknown reason is defaulted to a Pulled issue
            }

            return !reason.Equals(JiraConstants.IncompleteReason_PMO, StringComparison.InvariantCultureIgnoreCase)
                && !reason.Equals(JiraConstants.IncompleteReason_WrongTeam, StringComparison.InvariantCultureIgnoreCase)
                && !reason.Equals(JiraConstants.IncompleteReason_ThirdParty, StringComparison.InvariantCultureIgnoreCase);
        }


        public string GetIncompleteReasonAtSprintChange(DateTime sprintChangeDate)
        {
            string result = null;

            var histories = this.Changelog?.Histories;

            if (histories != null)
            {
                var historiesWithinWindow = histories.Where(h => h.Created >= sprintChangeDate && h.Created <= sprintChangeDate.AddHours(24))
                    .OrderByDescending(h => h.Created);

                JiraChangelogItemDetails latestReason 
                    = historiesWithinWindow.SelectMany(h => h.Items).Where(i => i.Field == "Incomplete Reason")?.FirstOrDefault();

                if (latestReason != null)
                {
                    result = latestReason.ToStr;
                }

            }

            return result;
        }

        private decimal? GetTotalHoursSpentInStatus(string status)
        {
            decimal? result = null;

            var histories = this.Changelog?.Histories;

            if (histories != null)
            {
                DateTime? startDateTime = null;
                DateTime? endDateTime = null;

                foreach (var h in histories.OrderBy(d => d.Created))
                {
                    var statusItems = h.Items.Where(i => i.Field == "status").ToList();

                    foreach (var i in statusItems)
                    {
                        if (i.ToStr == status)
                        {
                            startDateTime = h.Created;
                        }

                        if (i.FromString == status)
                        {
                            endDateTime = h.Created;
                        }
                    }

                    if (startDateTime != null && endDateTime != null)
                    {
                        if (result == null) result = 0;

                        decimal totalHours = (decimal)(endDateTime.Value - startDateTime.Value).TotalHours;

                        //count all days off to be removed from status time
                        do
                        {
                            startDateTime = startDateTime.Value.Date;

                            if (startDateTime.Value.Date.DayOfWeek == DayOfWeek.Saturday
                                ||
                               startDateTime.Value.Date.DayOfWeek == DayOfWeek.Sunday
                              )
                            {
                                totalHours -= 24;
                            }

                            startDateTime = startDateTime.Value.AddDays(1);
                        }
                        while (startDateTime < endDateTime);

                        result += totalHours;

                        startDateTime = null;
                        endDateTime = null;
                    }
                }
            }

            return result;
        }

        private decimal ParseEstimateHours(string rawHours)
        {
            decimal result = 0;

            if (!string.IsNullOrEmpty(rawHours))
            {

                if (!decimal.TryParse(rawHours, out result))
                {

                    if (rawHours.ToLower().Contains("h"))
                    {
                        decimal.TryParse(rawHours.ToLower().Replace("h", ""), out result);
                    }
                    else if (rawHours.ToLower().Contains("m") || rawHours.ToLower().Contains("min"))
                    {
                        rawHours = rawHours.ToLower().Replace("min", "").Replace("m", "");
                        decimal.TryParse(rawHours, out result);

                        result = result / 60;
                    }
                }
            }

            return result;
        }

        public class JiraIssueActor
        {

            public string Name { get; set; }

            public decimal TimeSpentSeconds { get; set; }

            public decimal Hours
            {
                get
                {
                    return this.TimeSpentSeconds / 60m / 60;
                }
            }

            public PersonRole Role { get; set; }

        }

        public class PulledIssueInfo
        {
            public bool isPulled { get; set; }

            public DateTime? PulledOn { get; set; }
        }



    }
}
