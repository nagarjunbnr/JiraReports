using JiraReports.Common;
using JiraReports.Reports.KeyPoints;
using JiraReports.Reports.KeyPoints.Predictability;
using JiraReports.Reports.KeyPoints.Productivity;
using JiraReports.Services;
using JiraReports.Services.Jira;
using JiraReports.Services.Jira.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JiraReports.Services.PersonnelService;

namespace JiraReports.Reports.Common.EffectiveValue
{
    public class SprintStatsCalculator
    {
        private string Project;
        private List<JiraIssue> allProjectsIssues;
        private List<(string Name, PersonRole Role)> sprintContributors;

        private IJiraIssueService issueService;
        private IPersonnelService personnelService;
        private IEffectiveWorkingDaysService workingDayService;
        private IJiraWorklogService worklogService;

        public SprintStatsCalculator(List<JiraIssue> allProjectsIssues, IJiraIssueService issueService, 
            IPersonnelService personnelService, IEffectiveWorkingDaysService workingDayService, IJiraWorklogService worklogService)
        {
            this.issueService = issueService;
            this.allProjectsIssues = allProjectsIssues;
            this.personnelService = personnelService;
            this.workingDayService = workingDayService;
            this.worklogService = worklogService;
        }


        public TeamSprintStats CalculateSprintStats(AgileSprint targetSprint)
        {
            var predictabilityResult = KeyPointsReport.CalculatePredictability(this.allProjectsIssues, new AgileSprint[] { targetSprint }.ToList(),
                this.issueService, new JiraConstants());

            var result = new TeamSprintStats()
            {
                Sprint = targetSprint.ParseSprintName().sprint,
                Team = targetSprint.ParseSprintName().project,
                PredictabilityScore = predictabilityResult.totals.FirstOrDefault()?.PredictabilityScore ?? 0
            };

            this.Project = allProjectsIssues.FirstOrDefault()?.Fields.Project.Key;

            this.sprintContributors = this.personnelService.GetProjectContributors(this.Project, result.Sprint.ToString());

            CalculateValueLoggedHours(targetSprint, result);

            AnalyzePredictability(predictabilityResult.details, result);

            CalculateFailureRate(result, targetSprint, allProjectsIssues.First());

            CalculateSprintResources(result, targetSprint);

            CalculateProductivity(result);


            return result;
        }

        private void AnalyzePredictability(List<PredictabilityBreakdown> details, TeamSprintStats stats)
        {
            var dictStats = stats.IssueStats.ToDictionary(d => d.Issue.Key, d => d);

            foreach (var predictabilityIssue in details)
            {
                dictStats.TryGetValue(predictabilityIssue.Issue.Key, out var issueStats);

                //there was never any work done on the issue before it was pulled
                //add it to the list with an empty worklog
                if (issueStats == null)
                {
                    issueStats = AddIssueStatsRecord(stats, predictabilityIssue.Issue, new List<JiraWorklog>(), isValueAdded: false);
                }

                issueStats.PredictabilityStatus = predictabilityIssue.Status;
                issueStats.IncompleteReason = predictabilityIssue.IncompleteReason;                
            }
        }

        private void CalculateProductivity(TeamSprintStats stats)
        {
            decimal totalProjectedHours = stats.AvailableResources.Sum(r => r.ProjectedHours);

            stats.ProductivityScore = totalProjectedHours > 0 ? Math.Round((stats.TotalLoggedHours / totalProjectedHours) * 100, 2) : 0;
        }

        private void CalculateValueLoggedHours(AgileSprint targetSprint, TeamSprintStats teamSprintData)
        {
            DateTime startDate = targetSprint.StartDate.Value.Date;
            DateTime endDate;
            if (targetSprint.FinishDate != null)
                endDate = targetSprint.FinishDate.Value.Date.AddDays(1).AddTicks(-1);
            else
                endDate = targetSprint.StartDate.Value.Date.AddDays(15);
            //add a 14 day grace period to give people time to fix their logs
            List<JiraWorklog> worklogs = this.worklogService.GetUpdatedWorklogs(startDate.AddDays(-14), endDate.AddDays(14));
            //List<JiraWorklog> worklogs = this.worklogService.GetUpdatedWorklogs(startDate, endDate);

            //use worklog StartDate for the actual worklog window since it's the StartDate that shows up on the UI
            worklogs = worklogs.Where(w => w.Started >= startDate && w.Started <= endDate).ToList();

            if (worklogs.Count == 0) return;

            string sprintNumber = targetSprint.ParseSprintName().sprint.ToString();
            //filter worklogs down by sprint members

            //this.SprintContributors = worklogs.Where(w => this.personnelService.GetPersonAllocation(w.Author.DisplayName, teamSprintData.Team, sprintNumber) > 0)
            //    .Select(d => d.Author.DisplayName).Distinct().ToList();

          //  worklogs = worklogs.Where(w => this.sprintContributors.Select(d => d.Name).Contains(w.Author.DisplayName)).Distinct().ToList();

           // List<JiraIssue> worklogIssues = this.issueService.GetIssues(worklogs.Select(w => w.IssueID).ToList(), JiraConstants.MainFields, true);

            //get all issues in the sprint to analyze for defects
            List<JiraIssue> sprintIssues = this.issueService.GetIssuesInSprint(targetSprint.ID, JiraConstants.MainFields);
            List<JiraIssue> worklogIssues = sprintIssues;
            //remove all worklogs with allocation oitside of the current project
            //only count work done on other projects if it wasn't planned for
            foreach (string contributor in sprintContributors.Select(c => c.Name))
            {
                List<ProjectSprintAllocation> outsideAllocations
                    = this.personnelService.GetAllPersonAllocations(contributor, sprintNumber)
                        .Where(a => a.Project.ToUpper() != this.Project.ToUpper() && a.Allocation > 0).ToList();

                List<string> plannedOutsideProjects = outsideAllocations.Select(a => a.Project.ToUpper()).Distinct().ToList();
                List<string> plannedOutsideIssues = worklogIssues.Where(i => plannedOutsideProjects.Contains(i.Fields.Project.Key.ToUpper()))
                    .Select(i => i.ID).ToList();

                //remove all worklogs for planned work outside the target sprint
                List<JiraWorklog> plannedOutsideWorklogs 
                    = worklogs.Where(w => w.Author.DisplayName == contributor && plannedOutsideIssues.Contains(w.IssueID)).ToList();
                List<string> plannedOutsideIssueIds = plannedOutsideWorklogs.Select(w => w.IssueID).Distinct().ToList();
                worklogIssues.RemoveAll(i => plannedOutsideIssueIds.Contains(i.ID));
            }

            var groupedWorklogs = worklogs.GroupBy(w => w.IssueID);

            foreach (var worklogIssue in worklogIssues)
            {
                worklogIssue.AnalyzeWorklog(this.issueService, this.personnelService, this.Project, sprintNumber);

                AddIssueStatsRecord(teamSprintData, worklogIssue, groupedWorklogs
                    .Where(w => w.Key == worklogIssue.ID).SelectMany(w => w).ToList());

                AnalyzeDefects(teamSprintData, sprintIssues);
            }
        }

        private SprintStatsIssue AddIssueStatsRecord(TeamSprintStats stats, JiraIssue i, 
            List<JiraWorklog> worklog, bool? isValueAdded = null)
        {
            int originalEstimateSeconds = 0;
            int.TryParse(i.Fields.TimeOriginalEstimate, out originalEstimateSeconds);

            var result = new SprintStatsIssue()
            {
                Issue = i,
                IsValueAdded = isValueAdded ?? IsValueAddedIssue(i, this.Project),
                Worklog = worklog,
                Components = String.Join(",", i.Fields.Components?.Select(c => c.Name) ?? Enumerable.Empty<string>()),
                Summary = i.Fields.Summary,

                EstimatedHours = Math.Round(originalEstimateSeconds > 0 ? originalEstimateSeconds / 60m / 60 : 0, 2)
            };

            stats.IssueStats.Add(result);

            return result;
        }

        private void AnalyzeDefects(TeamSprintStats stats, List<JiraIssue> sprintIssues)
        {
            foreach (var issueStats in stats.IssueStats)
            {
                var issueDefects = sprintIssues.Where(i => i.Fields.IssueType.Name == "Defect" 
                    && i.Fields.Parent.ID == issueStats.Issue.ID).ToList();

                issueStats.DefectCount = issueDefects.Count;
                
            }
        }

        private void CalculateFailureRate(TeamSprintStats teamSprintData, AgileSprint targetSprint, JiraIssue sampleProjectIssue)
        {
            List<JiraIssue> targetIssues = this.allProjectsIssues;//Where(i => i.Fields.Project.Key == sampleProjectIssue.Fields.Project.Key).ToList();

            List<JiraIssue> bugsDuringSprint = targetIssues.Where(i => i.IsCustomerReportedBug && i.Fields.Created >= targetSprint.StartDate 
                && i.Fields.Created <= targetSprint.EndDate).ToList();

            teamSprintData.FailureRateIssues = bugsDuringSprint;
            
        }

        private void CalculateSprintResources(TeamSprintStats teamSprintData, AgileSprint targetSprint)
        {
            if (targetSprint.StartDate != null && targetSprint.FinishDate != null)
            {
                foreach (var contributor in this.sprintContributors)
                {
                    decimal availability = this.workingDayService.GetPersonSprintAvailability(contributor.Name, this.Project,
                        targetSprint.ParseSprintName().sprint.ToString(), targetSprint.StartDate.Value, targetSprint.FinishDate.Value);

                    if (availability > 0)
                    {
                        decimal projectedHours = this.workingDayService.GetProjectedHours(contributor.Name, 
                            this.Project, targetSprint.ParseSprintName().sprint.ToString(),
                                targetSprint.StartDate.Value, targetSprint.FinishDate.Value);


                        teamSprintData.AvailableResources.Add(new AvailableResource()
                        {
                            Name = contributor.Name,
                            Availability = Math.Round(availability, 2),
                            Role = contributor.Role,
                            ProjectedHours = projectedHours
                        });
                    }
                }
            }

        }

        private bool IsValueAddedIssue(JiraIssue issue, string targetProject)
        {
            //issues outside the target project are considered non value
            if (issue.Fields.Project.Key.ToUpper() != targetProject.ToUpper()) return false;

            JiraIssue baseIssue = (issue.Fields.IssueType.Name == "Sub-task") ? issue.Fields.Parent : issue;

            return baseIssue.Fields.IssueType.Name == "Feature"
                || baseIssue.Fields.IssueType.Name == "Technical Debt"
                || baseIssue.Fields.IssueType.Name == "Defect"
                && (baseIssue.Fields.Resolution?.Name == "Done");
        }


    }
}
