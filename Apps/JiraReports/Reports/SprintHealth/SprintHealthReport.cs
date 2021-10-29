using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraReports.Common;
using JiraReports.Reports.Common.EffectiveValue;
using JiraReports.Reports.KeyPoints;
using JiraReports.Reports.KeyPoints.Predictability;
using JiraReports.Reports.KeyPoints.Productivity;
using JiraReports.Services;
using JiraReports.Services.Interfaces;
using JiraReports.Services.Jira;
using JiraReports.Services.Jira.Model;
using JiraReports.Services.Model;
using JiraReports.View.Reports;
using JiraReports.View.Reports.SprintHealth;
using JiraReports.ViewModel.Reports;
using JiraReports.ViewModel.Reports.SprintHealth;

namespace JiraReports.Reports.SprintHealth
{
    [SingleInstance(typeof(IReport))]
    [ReportOptionsView(optionsViewType: typeof(SprintHealthOptionsView), optionsViewModelType: typeof(SprintHealthOptionsViewModel))]
    [ReportDisplayView(displayViewType: typeof(ReportMultiGridView), displayViewModelType: typeof(SprintHealthGridViewModel))]
    public class SprintHealthReport : IReport
    {
        public string Name => "Sprint Analysis";

        public bool IsVisible
        {
            get
            {
                bool result = true;

#if Public
                    result = true;
#endif
#if Marketing
                    result = false;
#endif

                return result;
            }
        }

        private IJiraAgileService agileService;
        private IJiraIssueService issueService;
        private IEffectiveWorkingDaysService worklogService;
        private IPersonnelService personnelService;
        private ISprintService sprintService;
        private ISprintPointValueService pointValueService;
        private IJiraWorklogService workService;

        public SprintHealthReport(IJiraAgileService agileService, IJiraIssueService issueService,
            IEffectiveWorkingDaysService worklogService, IPersonnelService personnelService,
            ISprintService sprintService, ISprintPointValueService pointValueService, IJiraWorklogService workService)
        {
            this.agileService = agileService;
            this.issueService = issueService;
            this.worklogService = worklogService;
            this.personnelService = personnelService;
            this.sprintService = sprintService;
            this.pointValueService = pointValueService;
            this.workService = workService;
        }

        public void RunReport(ReportOptionsModel options, ReportDisplayModel display, IReportProgress progress)
        {
            SprintHealthGridViewModel grid = display as SprintHealthGridViewModel;
            SprintHealthOptionsViewModel model = options as SprintHealthOptionsViewModel;
            var constants = new JiraConstants();

            progress.ClearReportProgress();
            grid.ClearModels();

            if (model.SelectedBoard == null || model.SelectedSprintNumber == null)
            {
                return;
            }

            progress.SetStatus("Getting a list of boards (Step 1 of 4)...");

            AgileBoard targetBoard = agileService.GetDeliveryBoards().FirstOrDefault(b => b.ID == model.SelectedBoard.ID);

            if (targetBoard == null) return;

            JiraProjectItems targetProject = constants.Projects.FirstOrDefault(p => p.BoardName.Equals(targetBoard.Name,
                StringComparison.CurrentCultureIgnoreCase));

            if (targetProject == null)
            {
                progress.SetStatus("No matching project found");
                progress.Complete();
                return;
            }

            AgileSprint targetSprint = null;
            AgileSprint previousSprint = null;

            progress.SetStatus("Getting sprints for target project (Step 2 of 4)...");
            List<AgileSprint> boardSprints = this.agileService.GetSprintsForBoard(model.SelectedBoard.ID);

            //find the exact sprint being targeted
            foreach (AgileSprint sprint in boardSprints)
            {
                if (string.Equals(sprint.ParseSprintName().project, targetProject.SprintName, StringComparison.InvariantCultureIgnoreCase)
                    && sprint.ParseSprintName().sprint == model.SelectedSprintNumber.Value)
                {
                    targetSprint = sprint;

                    previousSprint = boardSprints.Where(s => s.ParseSprintName().sprint
                        == model.SelectedSprintNumber.Value - 1).FirstOrDefault();
                }
            }



            if (targetSprint?.StartDate == null)
            {
                if (previousSprint?.StartDate != null)
                {
                    //try to determine the start date by looking at the planning date of the previous sprint
                    DateTime prevReleaseDate = sprintService.GetTargetSprintReleaseDate(targetProject.Key,
                            previousSprint.ParseSprintName().sprint.ToString(), previousSprint.StartDate.Value);

                    DateTime prevPlanningDate = sprintService.GetTargetSprintPlanningDate(targetProject.Key,
                        previousSprint.ParseSprintName().sprint.ToString(), prevReleaseDate);

                    targetSprint.StartDate = prevPlanningDate.AddDays(1);
                }

                if (targetSprint?.StartDate == null)
                {
                    progress.SetStatus("No sprint start date set");
                    progress.Complete();
                    return;
                }
            }

            if (targetSprint?.FinishDate == null)
            {
                //estimate the end date if one isn't provided
                targetSprint.CompleteDate = sprintService.GetTargetSprintReleaseDate(targetProject.Key,
                    targetSprint.ParseSprintName().sprint.ToString(), targetSprint.StartDate.Value);
            }

            (_, string projectName, int sprintNumber) = targetSprint.ParseSprintName();

            progress.SetStatus("Getting issues in target project (Step 3 of 4)...");
            //grab all issues within the project and within the sprint start and end dates
            List<JiraIssue> allProjectIssues = this.issueService.GetIssuesInProject(targetProject.Key, JiraConstants.MainFields, true,
            updatedAfter: targetSprint.StartDate.Value.AddDays(-7), null, model.SelectedSprintNumber.Value.ToString());

            ////do not report on active sprints as they are still in flux
            //if (!targetSprint.State.Equals("active", StringComparison.CurrentCultureIgnoreCase)) return;

            progress.SetStatus("Calculating metrics (Step 4 of 4)...");



            //show all issue breakdown
            var sprintStatsCalculator = new SprintStatsCalculator(allProjectIssues, this.issueService,
                this.personnelService, this.worklogService, this.workService);

            var sprintStats = sprintStatsCalculator.CalculateSprintStats(targetSprint);
            var sprintHealthIssues = new List<SprintHealthIssue>();

            foreach (var issueStats in sprintStats.IssueStats)
            {
                var sprintHealthIssue = new SprintHealthIssue();

                if (issueStats.Issue.Key.Equals("MOB-2987"))
                {
                    Console.WriteLine(issueStats);
                }
                sprintHealthIssue.IssueKey = issueStats.Issue.Key;
                sprintHealthIssue.IssueId = issueStats.Issue.ID;
                sprintHealthIssue.IssueType = issueStats.Issue.Fields.IssueType.Name;
                sprintHealthIssue.IssueBucket = GetIssueBucket(sprintHealthIssue.IssueType);
                sprintHealthIssue.Sprint = issueStats.Issue.Fields.Sprint.Contains("XRMCore") ? issueStats.Issue.Fields.Sprint.Replace("XRMCore", "XRM Core")
                                           : (
                                                issueStats.Issue.Fields.Sprint.Contains("XRMInt") ? issueStats.Issue.Fields.Sprint.Replace("XRMInt", "XRM Integration")
                                           : (
                                                issueStats.Issue.Fields.Sprint.Contains("COE Reporting") ? issueStats.Issue.Fields.Sprint.Replace("COE Reporting", "COE")
                                                : issueStats.Issue.Fields.Sprint)
                                             );
                sprintHealthIssue.PredictabilityStatus = issueStats.PredictabilityStatus;
                sprintHealthIssue.IncompleteReason = issueStats.IncompleteReason;
                sprintHealthIssue.Resolution = issueStats.Resolution;
                sprintHealthIssue.Components = issueStats.Components;
                sprintHealthIssue.EpicLink = issueStats.Issue.Fields.Epic;
                sprintHealthIssue.IssueLinks = issueStats.Issue.Fields.IssueLinks;
                if (!sprintHealthIssue.EpicLink.Equals(String.Empty))
                {
                    sprintHealthIssue.EpicInvestmentCategory = issueService.GetEpicInvestmentCategory(issueStats.Issue.Fields.Epic) ?? "Unknown";
                }
                else
                {
                    if (sprintHealthIssue.IssueLinks.Count == 0)
                    {
                        if (sprintHealthIssue.IssueType.Equals("Schedule Item") || sprintHealthIssue.IssueType.Equals("Planning"))
                        {
                            sprintHealthIssue.EpicInvestmentCategory = "Sustainment";
                        }
                    }

                    else if (sprintHealthIssue.IssueLinks.Count > 0)
                    {

                        foreach (JiraIssueLink issuelink in sprintHealthIssue.IssueLinks)
                        {
                            if (issuelink.OutwardIssue != null)
                            {
                                if (issuelink.OutwardIssue.Fields.IssueType.Name == "Planning")
                                {
                                    sprintHealthIssue.EpicInvestmentCategory = "Sustainment";
                                }
                                else if (issuelink.OutwardIssue.Fields.IssueType.Name == "Feature" || issuelink.OutwardIssue.Fields.IssueType.Name == "Inquiry"
                                    || issuelink.OutwardIssue.Fields.IssueType.Name == "Bug")
                                {
                                    var issue = issueService.GetIssue(issuelink.OutwardIssue.Key, JiraConstants.MainFields, true);
                                    if (issue?.Fields != null && !String.IsNullOrEmpty(issue.Fields.Epic))
                                    {
                                        sprintHealthIssue.EpicLink = issue.Fields.Epic;
                                        sprintHealthIssue.EpicInvestmentCategory = issueService.GetEpicInvestmentCategory(issue?.Fields?.Epic) ?? "Unknown";
                                    }
                                }
                            }
                            else if (issuelink.InwardIssue != null)
                            {
                                if (issuelink.InwardIssue.Fields.IssueType.Name == "Planning")
                                {
                                    sprintHealthIssue.EpicInvestmentCategory = "Sustainment";
                                }

                                else if (issuelink.InwardIssue.Fields.IssueType.Name == "Feature" || issuelink.InwardIssue.Fields.IssueType.Name == "Inquiry"
                                    || issuelink.InwardIssue.Fields.IssueType.Name == "Bug")
                                {
                                    var issue = issueService.GetIssue(issuelink.InwardIssue.Key, JiraConstants.MainFields, true);
                                    if (issue?.Fields != null && !String.IsNullOrEmpty(issue.Fields.Epic))
                                    {
                                        sprintHealthIssue.EpicLink = issue.Fields.Epic;
                                        sprintHealthIssue.EpicInvestmentCategory = issueService.GetEpicInvestmentCategory(issue?.Fields?.Epic) ?? "Unknown";
                                    }
                                }
                            }
                        }

                    }
                }
                
                if (String.IsNullOrEmpty(sprintHealthIssue.EpicInvestmentCategory) || sprintHealthIssue.EpicInvestmentCategory == "Unknown")
                {
                    sprintHealthIssue.EpicInvestmentCategory = "Sustainment";
                }

                sprintHealthIssue.Eng = string.Join(", ",
                    issueStats.GetEngineeringContributors(this.personnelService, targetProject.Key, sprintNumber.ToString()));
                sprintHealthIssue.QA = string.Join(", ",
                    issueStats.GetQAContributors(this.personnelService, targetProject.Key, sprintNumber.ToString()));
                sprintHealthIssue.EstimatedHours = issueStats.EstimatedHours;
                sprintHealthIssue.TotalLoggedHours = Math.Round(issueStats.LoggedHours, 2);
                sprintHealthIssue.LoggedEngHours = Math.Round(issueStats.GetLoggedEngHours(this.personnelService, targetProject.Key, sprintNumber.ToString()), 2);
                sprintHealthIssue.LoggedEngDevHours = Math.Round(issueStats.GetLoggedEngDevHours(this.personnelService, targetProject.Key, sprintNumber.ToString(), targetSprint.StartDate.Value, targetSprint.EndDate.Value), 2);
                sprintHealthIssue.LoggedEngDefectFixHours = Math.Round(issueStats.GetLoggedEngDefectFixHours(this.personnelService, targetProject.Key, sprintNumber.ToString(), targetSprint.StartDate.Value, targetSprint.EndDate.Value), 2);
                // if (issueStats.IssueType == "Planning")
                //  {
                sprintHealthIssue.LoggedEngPlanHours = Math.Round(issueStats.GetLoggedEngPlanHours(this.personnelService, targetProject.Key, sprintNumber.ToString(), targetSprint.StartDate.Value, targetSprint.EndDate.Value), 2);
                // }
                // else
                //  {
                //      sprintHealthIssue.LoggedEngDefectFixHours += Math.Round(issueStats.GetLoggedEngPlanHours(this.personnelService, targetProject.Key, sprintNumber.ToString(), targetSprint.StartDate.Value, targetSprint.EndDate.Value), 2);
                // }

                sprintHealthIssue.LoggedQATestHours = Math.Round(issueStats.GetLoggedQATestHours(this.personnelService, targetProject.Key, sprintNumber.ToString(), targetSprint.StartDate.Value, targetSprint.EndDate.Value), 2);
                sprintHealthIssue.LoggedQARegressionHours = Math.Round(issueStats.GetLoggedQARegressionHours(this.personnelService, targetProject.Key, sprintNumber.ToString(), targetSprint.StartDate.Value, targetSprint.EndDate.Value), 2);
                //   if (issueStats.IssueType == "Planning")
                // {
                sprintHealthIssue.LoggedQAPlanHours = Math.Round(issueStats.GetLoggedQAPlanHours(this.personnelService, targetProject.Key, sprintNumber.ToString(), targetSprint.StartDate.Value, targetSprint.EndDate.Value), 2);
                //  }
                //  else
                //  {
                //      sprintHealthIssue.LoggedQARegressionHours += Math.Round(issueStats.GetLoggedQAPlanHours(this.personnelService, targetProject.Key, sprintNumber.ToString(), targetSprint.StartDate.Value, targetSprint.EndDate.Value), 2);
                //  }
                // sprintHealthIssue.LoggedEngPlanHours = Math.Round(issueStats.GetLoggedEngPlanHours(this.personnelService, targetProject.Key, sprintNumber.ToString(), targetSprint.StartDate.Value, targetSprint.EndDate.Value), 2);
                sprintHealthIssue.LoggedQAHours = Math.Round(issueStats.GetLoggedQAHours(this.personnelService, targetProject.Key, sprintNumber.ToString()), 2);
                sprintHealthIssue.TotalHoursInReview = Math.Round(issueStats.Issue.GetTotalHoursInReview() ?? 0, 2);
                sprintHealthIssue.TotalHoursInCodeMerge = Math.Round(issueStats.Issue.GetTotalHoursInCodeMerge() ?? 0, 2);
                sprintHealthIssue.DefectCount = issueStats.DefectCount;

                if (!string.IsNullOrEmpty(issueStats.Issue.Fields.OriginalEngHours))
                {
                    sprintHealthIssue.EngEstimate = issueStats.Issue.EngineeringEstimate;
                }

                if (!string.IsNullOrEmpty(issueStats.Issue.Fields.OriginalQAHours))
                {
                    sprintHealthIssue.QAEstimate = issueStats.Issue.QAEstimate;
                }

                sprintHealthIssues.Add(sprintHealthIssue);
            }

            grid.SetNextModel("Issues", sprintHealthIssues);

            List<JiraWorklog> allWorklogs = sprintStats.IssueStats.SelectMany(i => i.Worklog).ToList();
            var allPersonStats = new List<SprintHealthPersonalStats>();

            foreach (var availableResource in sprintStats.GetAvailableResources())
            {
                var personStats = new SprintHealthPersonalStats();

                personStats.Name = availableResource.Name;
                personStats.ProjectedHours = availableResource.ProjectedHours;
                personStats.Role = availableResource.Role;
                personStats.LoggedHours = allWorklogs.Where(w => w.Author.DisplayName == personStats.Name).Sum(w => w.Hours);

                allPersonStats.Add(personStats);
            }

            grid.SetNextModel("Person Totals", allPersonStats.Select(s => new
            {
                Name = s.Name,
                ProjectedHours = s.ProjectedHours,
                Role = s.Role.ToString(),
                LoggedHours = s.LoggedHours
            }));

            //calculate key dates
            if (targetSprint.StartDate.HasValue)
            {
                DateTime targetReleaseDate = this.sprintService.GetTargetSprintReleaseDate(targetProject.SprintName, sprintNumber.ToString(),
                    targetSprint.StartDate.Value);

                DateTime targetSprintPlanningDate = this.sprintService.GetTargetSprintPlanningDate(targetProject.SprintName, sprintNumber.ToString(),
                    targetReleaseDate);

                grid.SetNextModel("Key Dates", new SprintHealthDates[] {
                new SprintHealthDates {
                    StartDate = targetSprint.StartDate.Value.ToShortDateString(),
                    EndDate = targetSprint.FinishDate?.ToShortDateString(),
                    TargetReleaseDate = targetReleaseDate.ToShortDateString(),
                    TargetSprintPlanningDate = targetSprintPlanningDate.ToShortDateString()
                } });
            }

            var sprintTotals = new SprintHealthHourTotals()
            {
                ProjectedEngHours = allPersonStats.Where(s => s.Role == PersonRole.Eng).Sum(s => s.ProjectedHours) ?? 0,
                ProjectedQAHours = allPersonStats.Where(s => s.Role == PersonRole.QA).Sum(s => s.ProjectedHours) ?? 0,
                PlannedEngHours = sprintHealthIssues.Sum(s => s.EngEstimate),
                PlannedNonInquiryEngHours = sprintHealthIssues.Where(s => s.IssueType != "Inquiry").Sum(s => s.EngEstimate),
                PlannedQAHours = sprintHealthIssues.Sum(s => s.QAEstimate),
                PlannedHours = sprintHealthIssues.Sum(i => i.EstimatedHours),
                LoggedHours = allPersonStats.Sum(s => s.LoggedHours),
                LoggedEngHours = allPersonStats.Where(s => s.Role == PersonRole.Eng).Sum(s => s.LoggedHours),
                LoggedQAHours = allPersonStats.Where(s => s.Role == PersonRole.QA).Sum(s => s.LoggedHours),
            };

            grid.SetNextModel("Sprint Totals", new SprintHealthHourTotals[] { sprintTotals });

#if !Public

            grid.SetNextModel("Sprint Value Totals", new TeamSprintStatsView[] { sprintStats.GetValueView() });
            grid.SetNextModel("Sprint Value Issues", sprintStats.GetEffectiveValueIssues());
            grid.SetNextModel("Failure Rate Issues", sprintStats.GetFailureRateIssues());
            grid.SetNextModel("Available Resources", sprintStats.GetAvailableResources());

#endif

            progress.SetStatus("Done");
            progress.Complete();
        }

        private string GetIssueBucket(string issueType)
        {
            var result = "";

            switch (issueType)
            {
                case "Inquiry":
                case "Feature":
                case "Dev/QA":
                case "Test/Testing":
                    result = "Innovation";
                    break;
                case "Migration":
                    result = "Migration Support";
                    break;

                case "Planning":
                case "QA":
                case "Task":
                case "Helpdesk":
                    result = "Operation";
                    break;
                case "Defect":
                    result = "Defect";
                    break;

                case "Bug":
                    result = "Quality";
                    break;

                case "Ad-hoc":
                case "Documentation":
                case "Schedule Item":
                    result = "Maintenance";
                    break;
                case "Technical Debt":
                    result = "Technical Debt";
                    break;
            }

            return result;
        }
    }

}
