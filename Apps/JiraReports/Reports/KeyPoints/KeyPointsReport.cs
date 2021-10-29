using JiraReports;
using JiraReports.Common;
using JiraReports.Reports.Common.EffectiveValue;
using JiraReports.Reports.KeyPoints.Predictability;
using JiraReports.Reports.KeyPoints.Productivity;
using JiraReports.Reports.KeyPoints.ViewModel;
using JiraReports.Services;
using JiraReports.Services.Jira;
using JiraReports.Services.Jira.Model;
using JiraReports.View.Reports;
using JiraReports.View.Reports.KeyPoints;
using JiraReports.ViewModel.Reports;
using JiraReports.ViewModel.Reports.KeyPoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JiraReports.Reports.KeyPoints
{
    [SingleInstance(typeof(IReport))]
    [ReportOptionsView(optionsViewType: typeof(KeyPointsOptionsView), optionsViewModelType: typeof(KeyPointsOptionsViewModel))]
    [ReportDisplayView(displayViewType: typeof(KeyPointsMultiGridView), displayViewModelType: typeof(KeyPointsGridViewModel))]
    public class KeyPointsReport : IReport
    {
        public string Name => "KPI";

        public bool IsVisible
        {
            get
            {
                bool result = true;

                #if Public
                    result = false;
                #endif
                #if Marketing
                    result = false;
                #endif

                return result;
            }
        }


        private IJiraAgileService agileService;
        private IJiraGitService jiraGitService;
        private IJiraProjectService projectService;
        private IJiraIssueService issueService;
        private IEffectiveWorkingDaysService workDaysSvc;
        private IPersonnelService personnelService;
        private IJiraWorklogService worklogService;

        private JiraConstants jiraConstants;

        public KeyPointsReport(IJiraAgileService agileService, IJiraGitService jiraGitService, IJiraProjectService projectService,
            IJiraIssueService issueService, IEffectiveWorkingDaysService workDaysSvc, IPersonnelService personnelService,
            IJiraWorklogService worklogService, JiraConstants jiraConstants)
        {
            this.agileService = agileService;
            this.jiraGitService = jiraGitService;
            this.projectService = projectService;
            this.issueService = issueService;
            this.workDaysSvc = workDaysSvc;
            this.personnelService = personnelService;
            this.worklogService = worklogService;
            this.jiraConstants = jiraConstants;
        }

        public void RunReport(ReportOptionsModel options, ReportDisplayModel display, IReportProgress progress)
        {
            int sprintsToView = 6;
            var constants = new JiraConstants();

            KeyPointsGridViewModel kpiDisplay = display as KeyPointsGridViewModel;
            KeyPointsOptionsViewModel kpiOptions = options as KeyPointsOptionsViewModel;

            progress.ClearReportProgress();
            kpiDisplay.ClearModels();

            progress.SetStatus("Getting a list of boards...");
            List<AgileBoard> relevantBoards = agileService.GetDeliveryBoards();

            if (kpiOptions.SelectedBoard?.ID != null)
            {
                relevantBoards = relevantBoards.Where(b => b.ID == kpiOptions.SelectedBoard.ID).ToList();
            }

            progress.SetStatus("Getting Sprints...");
            progress.ClearReportProgress();
            progress.SetReportTotal(relevantBoards.Count);
            List<AgileSprint> allSprints = new List<AgileSprint>();
            foreach (AgileBoard board in relevantBoards)
            {
                allSprints.AddRange(agileService.GetSprintsForBoard(board.ID));
                progress.IncremenetReportProgress();
            }

            if (kpiOptions.SelectedSprintNumber?.Value != null)
            {
                allSprints = allSprints.Where(s => s.ParseSprintName().sprint
                    == kpiOptions.SelectedSprintNumber.Value).ToList();
            }

            List<AgileSprint> lastNSprints = allSprints.Where(s => s.State == "closed")
                .Distinct((a, b) => a.ID == b.ID)
                .GroupBy(s => s.OriginBoardID)
                .SelectMany(s => s.OrderByDescending(sp => sp.StartDate).Take(sprintsToView))
                .ToList();

            progress.SetStatus("Getting Projects...");
            progress.ClearReportProgress();
            List<JiraProject> projects = this.projectService.GetProjects();

            progress.ClearReportProgress();

            var kpiSprintTotals = new List<KeyPointsSprintTotals>();

            var relevantProjects = new List<JiraProject>();

            //filter down projects to only the relevant ones for the KPI
            foreach (var project in projects)
            {
                var projectConstants = constants.Projects.FirstOrDefault(p => p.Key == project.Key);

                //skip if there are no constants for this project
                //this means the project is irrelevant to KPIs
                if (projectConstants == null) continue;

                //skip if the relevant boards don't contain this project
                if (!relevantBoards.Select(b => b.Name).Contains(projectConstants.BoardName)) continue;

                relevantProjects.Add(project);
            }

            progress.SetReportTotal(relevantProjects.Count);

            var teamSprintStats = new List<TeamSprintStatsView>();

            foreach (var project in relevantProjects)
            {

                var projectConstants = constants.Projects.FirstOrDefault(p => p.Key == project.Key);

                var projectSprints = lastNSprints.Where(s => s.SprintProject?.ToUpper() == projectConstants.SprintName.ToUpper()).ToList();

                //skip if the project has no sprints
                if (projectSprints.Count == 0) continue;

                progress.SetStatus($"Getting Project Issues: {projectConstants.FriendlyName}");

                var earliestProjectSprintStart = projectSprints.Min(s => s.StartDate.Value);

                string sprintId = string.Empty;

                List<JiraIssue> allProjectIssues = this.issueService.GetIssuesInProject(projectConstants.Key, JiraConstants.MainFields, true,
                updatedAfter: earliestProjectSprintStart.AddDays(-7),null,sprintId);

                var sprintStatsCalculator = new SprintStatsCalculator(allProjectIssues, this.issueService, 
                    this.personnelService, this.workDaysSvc, this.worklogService);

                //loop through the sprints and calculate statistics
                foreach (var projectSprint in projectSprints)
                {
                    var sprintTotals = new KeyPointsSprintTotals();
                    var parsedSprint = projectSprint.ParseSprintName();

                    progress.SetStatus($"Calculating Sprint KPIs: {parsedSprint.sprint} {projectConstants.FriendlyName}");

                    TeamSprintStats projectStats = sprintStatsCalculator.CalculateSprintStats(projectSprint);

                    sprintTotals.Team = projectConstants.SprintName;
                    sprintTotals.Sprint = parsedSprint.sprint;
                    sprintTotals.Predictability = projectStats.PredictabilityScore;
                    sprintTotals.Productivity = projectStats.ProductivityScore;
                    sprintTotals.SprintValue = Math.Round(projectStats.SprintEffectiveValue, 2);


                    kpiSprintTotals.Add(sprintTotals);
                    teamSprintStats.Add(projectStats.GetValueView());
                }
                progress.IncremenetReportProgress();

            }

            //calculate sprint averages
            foreach(var sprintGroup in kpiSprintTotals.GroupBy(s => s.Sprint))
            {
                //add All global average
                kpiSprintTotals.Add(new KeyPointsSprintTotals()
                {
                    Team = "All",
                    Sprint = sprintGroup.Key,
                    Productivity = sprintGroup.Average(g => g.Productivity),
                    Predictability = sprintGroup.Average(g => g.Predictability),
                    SprintValue = sprintGroup.Average(g => g.SprintValue)
                });;
            }

            kpiDisplay.SetNextModel("Team KPI Summary", kpiSprintTotals);
            kpiDisplay.KPISprintTotals = kpiSprintTotals;

            kpiDisplay.SetNextModel("Team Sprint Value Stats", teamSprintStats);
            kpiDisplay.TeamSprintStats = teamSprintStats;

            progress.SetStatus("Done.");
            progress.Complete();
        }

        private void CalculatePredictability(KeyPointsGridViewModel kpiDisplay, 
            List<JiraIssue> allIssues, List<AgileSprint> allSprints)
        {
            KeyPointsReport.CalculatePredictability(allIssues, allSprints, this.issueService, this.jiraConstants, kpiDisplay);
        }

        public static (List<PredictabilityData> totals, List<PredictabilityBreakdown> details) 
            CalculatePredictability(List<JiraIssue> allIssues, List<AgileSprint> allSprints, 
                IJiraIssueService issueService, JiraConstants jiraConstants, KeyPointsGridViewModel kpiDisplay = null)
        {

            var kpiSprints = new Dictionary<string, KPISprint>();

            //add in all issues that are known to have been in the sprints
            foreach (var sprint in allSprints)
            {
                List<JiraIssue> issues = issueService.GetIssuesInSprint(sprint.ID, JiraConstants.MainFields);
                    //new string[] { "issuetype", "project", "timeoriginalestimate",
                    //               "customfield_11800", //Incomplete
                    //               "customfield_12000", //IncompleteReason
                    //             });

                KPISprint kpiSprint = GetKPISprint(sprint, kpiSprints, allSprints);

                foreach (var i in issues)
                {
                    JiraIssue fullDataIssue = allIssues.FirstOrDefault(d => d.Key == i.Key);

                    if (fullDataIssue != null)
                    {
                        kpiSprint.AddIssueInSprint(fullDataIssue);

                        //If the isue is marked as Incomplete, it assumed to have been pulled from the sprint
                        //Because the underlying work wasn't finished
                        if (fullDataIssue.IsIncomplete)
                        {
                            kpiSprint.AddIssueLeftSprint(fullDataIssue);
                        }
                        else if(!string.IsNullOrEmpty(fullDataIssue.Fields.ResolutionDate))
                        {
                            kpiSprint.AddIssueCompletedInSprint(fullDataIssue);
                        }
                    }
                }
            }

            //analyze change log to see which cases left sprints
            foreach (var issue in allIssues)
            {
                var histories = issue.Changelog.Histories;

                foreach (var h in histories)
                {
                    var sprintItems = h.Items.Where(i => i.Field == "Sprint").ToList();

                    foreach (var sprintItem in sprintItems)
                    {
                        //any sprint item transition that where the From Sprint doesn't appear in the To Sprint
                        //is counted as a case that had to leave the sprint it was in
                        if (sprintItem.From != null && sprintItem.To != null
                            &&
                            !sprintItem.To.Contains(sprintItem.From))
                        {

                            AgileSprint fromSprint = allSprints.FirstOrDefault(s => s.ID == sprintItem.From);

                            //the from sprint must be within the analyzed scope
                            if (fromSprint != null)
                            {
                                KPISprint fromKPISprint = GetKPISprint(fromSprint, kpiSprints, allSprints);

                                if (fromKPISprint != null)
                                {
                                    //count the miss only if the transition occured after the From sprint originally started
                                    if (h.Created >= fromSprint.StartDate)
                                    {
                                        fromKPISprint.AddIssueInSprint(issue); //issue was in sprint but then was pulled
                                        fromKPISprint.AddIssueLeftSprint(issue);
                                    }
                                }
                            }
                        }
                    }
                }


            }

            var predictabilityData = new List<PredictabilityData>();

            foreach (var kpiSprint in kpiSprints.Values)
            {
                var parsedSprint = kpiSprint.Sprint.ParseSprintName();

                if (parsedSprint.matched && kpiSprint.IssuesInSprint.Count > 0)
                {
                    var data = new PredictabilityData()
                    {
                        Project = parsedSprint.project,
                        Sprint = parsedSprint.sprint,
                        SprintId = kpiSprint.SprintId,
                        SprintStartDate = kpiSprint.Sprint.StartDate?.ToShortDateString(),
                        PredictabilityScore = Math.Round(100 - ((kpiSprint.ValidIssuesLeftSprint.Sum(i => i.OriginalEstimateHours ?? 0)
                            / (decimal)kpiSprint.IssuesInSprint.Sum(i => i.OriginalEstimateHours)) * 100), 2),
                        HoursPlanned = kpiSprint.IssuesInSprint.Sum(i => i.OriginalEstimateHours ?? 0),
                        HoursCompleted = kpiSprint.IssuesCompletedInSprint.Sum(i => i.OriginalEstimateHours ?? 0),
                        HoursPulled = kpiSprint.ValidIssuesLeftSprint.Sum(i => i.OriginalEstimateHours ?? 0)
                    };

                    predictabilityData.Add(data);

                }
            }

            var allPredictability = predictabilityData
                .Where(p => jiraConstants.Projects.Any(jp => String.Compare(jp.SprintName, p.Project, true) == 0))
                .GroupBy(p => p.Sprint);

            foreach (var predictabilityPerSprint in allPredictability)
            {
                PredictabilityData data = new PredictabilityData()
                {
                    Project = "All",
                    Sprint = predictabilityPerSprint.Key,
                    SprintId = "",
                    SprintStartDate = predictabilityPerSprint.First().SprintStartDate,
                    HoursPlanned = predictabilityPerSprint.Sum(p => p.HoursPlanned),
                    HoursCompleted = predictabilityPerSprint.Sum(p => p.HoursCompleted),
                    HoursPulled = predictabilityPerSprint.Sum(p => p.HoursPulled)
                };

                data.PredictabilityScore = Math.Round(100 - ((data.HoursPulled / (decimal)data.HoursPlanned) * 100), 2);

                predictabilityData.Add(data);
            }

            if (kpiDisplay != null)
            {
                kpiDisplay.SetNextModel("[Predictability] Sprints", predictabilityData);
                //kpiDisplay.PredictabilityData = predictabilityData;
            }

            var predictabilityBreakdown = new List<PredictabilityBreakdown>();

            foreach (var kpiSprint in kpiSprints.Values)
            {
                var parsedSprint = kpiSprint.Sprint.ParseSprintName();

                if (parsedSprint.matched)
                {
                    foreach (var i in kpiSprint.IssuesCompletedInSprint)
                    {
                        predictabilityBreakdown.Add(new PredictabilityBreakdown()
                        {
                            SprintId = kpiSprint.SprintId,
                            Project = parsedSprint.project,
                            Sprint = parsedSprint.sprint,
                            IssueName = i.Key,
                            IssueType = i.Fields.IssueType.Name,
                            Status = "Completed",
                            Issue = i
                        });
                    }


                    foreach (var i in kpiSprint.IssuesLeftSprint)
                    {
                        predictabilityBreakdown.Add(new PredictabilityBreakdown()
                        {
                            SprintId = kpiSprint.SprintId,
                            Project = parsedSprint.project,
                            Sprint = parsedSprint.sprint,
                            IssueName = i.Key,
                            IssueType = i.Fields.IssueType.Name,
                            Status = "Pulled",
                            IncompleteReason = i.GetIncompleteReason(kpiSprint.Sprint),
                            IsPulled = true,
                            Issue = i
                        });
                    }
                }
            }

            if (kpiDisplay != null)
            {

                kpiDisplay.SetNextModel("[Predictability] Teams", predictabilityData.GroupBy(d => d.Project, d => d)
                    .Select(p => new PredictabilityTeams()
                    {
                        Project = p.Key,
                        PredictabilityScore = p.Average(v => v.PredictabilityScore)
                    }));

                //kpiDisplay.SetNextModel("[Predictability] Cases", predictabilityBreakdown);
            }

            return (predictabilityData, predictabilityBreakdown);
        }


        private List<JiraIssue> GetIssues(IReportProgress progress, List<string> issueIDs, string[] fields)
        {
            int issuesPerRequest = 100;
            progress.ClearReportProgress();
            progress.SetReportTotal((int)Math.Ceiling((decimal)issueIDs.Count / issuesPerRequest));

            List<JiraIssue> issues = new List<JiraIssue>();
            for (int i = 0; i < issueIDs.Count; i += issuesPerRequest)
            {
                issues.AddRange(this.issueService.GetIssues(issueIDs.Skip(i).Take(issuesPerRequest).ToList(), fields, true));
                progress.IncremenetReportProgress();
            }

            return issues;
        }

        private static KPISprint GetKPISprint(AgileSprint baseSprint, Dictionary<string, KPISprint> sprints,
            List<AgileSprint> allSprints)
        {
            KPISprint sprint = null;

            if (allSprints.Count(d => d.ID == baseSprint.ID) > 0)
            {
                sprints.TryGetValue(baseSprint.ID, out sprint);

                if (sprint == null)
                {
                    sprint = new KPISprint(baseSprint);
                    sprints.Add(baseSprint.ID, sprint);
                }
            }

            return sprint;
        }

        private class KPISprint
        {
            public string SprintId
            {
                get
                {
                    return this.Sprint.ID;
                }
            }

            public List<JiraIssue> IssuesLeftSprint { get; set; }

            public List<JiraIssue> ValidIssuesLeftSprint
            {
                get
                {
                    return this.IssuesLeftSprint.Where(i => i.IsValidIncompleteReason(this.Sprint)).ToList();
                }
            }

            public List<JiraIssue> IssuesInSprint { get; set; }

            public List<JiraIssue> IssuesCompletedInSprint { get; set; }

            public AgileSprint Sprint { get; set; }

            public KPISprint(AgileSprint sprint)
            {
                this.Sprint = sprint;
                this.IssuesLeftSprint = new List<JiraIssue>();
                this.IssuesInSprint = new List<JiraIssue>();
                this.IssuesCompletedInSprint = new List<JiraIssue>();
            }

            public void AddIssueInSprint(JiraIssue issue)
            {
                if (IsIssueApplicable(issue) && this.IssuesInSprint.Count(i => i.ID == issue.ID) == 0)
                {
                    this.IssuesInSprint.Add(issue);
                }
            }

            public void AddIssueLeftSprint(JiraIssue issue)
            {
                if (IsIssueApplicable(issue) && this.IssuesLeftSprint.Count(i => i.ID == issue.ID) == 0
                    && this.IssuesCompletedInSprint.Count(i => i.ID == issue.ID) == 0)
                {
                    this.IssuesLeftSprint.Add(issue);
                }
            }

            public void AddIssueCompletedInSprint(JiraIssue issue)
            {
                if (IsIssueApplicable(issue) && this.IssuesCompletedInSprint.Count(i => i.ID == issue.ID) == 0)
                {
                    this.IssuesCompletedInSprint.Add(issue);
                }
            }

            public bool IsIssueApplicable(JiraIssue issue)
            {
                return issue.Fields.IssueType.Name == "Feature" || issue.Fields.IssueType.Name == "Bug" || issue.Fields.IssueType.Name == "Technical Debt";
            }
        }
    }
}
