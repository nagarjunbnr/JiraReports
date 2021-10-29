using JiraReports.Reports.TimeTracking;
using JiraReports.Services;
using JiraReports.Services.Jira;
using JiraReports.Services.Jira.Model;
using JiraReports.View.Reports;
using JiraReports.View.Reports.CapEx;
using JiraReports.ViewModel.Reports;
using JiraReports.ViewModel.Reports.CapEx;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.CapEx
{
    public class CapExReport
    {
        [SingleInstance(typeof(IReport))]
        [ReportOptionsView(optionsViewType: typeof(CapExOptionsView), optionsViewModelType: typeof(CapExOptionsViewModel))]
        [ReportDisplayView(displayViewType: typeof(ReportMultiGridView), displayViewModelType: typeof(CapExGridViewModel))]
        public class TimeTrackingReport : IReport
        {
            private IJiraWorklogService worklogService;
            private IJiraIssueService jiraIssueService;
            private IServiceLocator serviceLocator;

            private string[] fields = new string[] { "created", "summary", "components", "issuetype", "fixVersions", "components", "project", "parent",
            "customfield_11500", "customfield_10304", "customfield_10100", "timeoriginalestimate", "resolution", "resolutiondate" };

            public string Name => "CapEx";

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


            public TimeTrackingReport(IServiceLocator serviceLocator, IJiraWorklogService worklogService,
                IJiraIssueService jiraIssueService)
            {
                this.serviceLocator = serviceLocator;
                this.worklogService = worklogService;
                this.jiraIssueService = jiraIssueService;
            }

            public void RunReport(ReportOptionsModel options, ReportDisplayModel display, IReportProgress progress)
            {
                CapExGridViewModel trackingDisplay = display as CapExGridViewModel;
                CapExOptionsViewModel trackingOptions = options as CapExOptionsViewModel;
                progress.ClearReportProgress();
                trackingDisplay.ClearModels();



                DateTime startDate = trackingOptions.StartDate.Value;
                DateTime endDate = trackingOptions.EndDate ?? DateTime.UtcNow;

                progress.SetStatus("Loading Worklogs (Step 1 of 4)...");
                List<JiraWorklog> worklogItems = GetWorklogs(progress, startDate, endDate);

                List<string> issueIDs = worklogItems.GroupBy(w => w.IssueID).Select(i => i.Key).ToList();

                progress.SetStatus("Loading Issues (Step 2 of 4)...");
                Dictionary<string, JiraIssue> issues = GetIssues(progress, issueIDs, fields);

                progress.SetStatus("Loading Parent Issues (Step 3 of 4)...");
                List<string> parentIssueIDs = issues.Values.Where(i => String.IsNullOrWhiteSpace(i.Fields.Epic))
                    .Select(i => i.Fields.Parent?.ID).Where(i => !String.IsNullOrWhiteSpace(i)).Distinct().ToList();
                Dictionary<string, JiraIssue> parentIssues = GetIssues(progress, parentIssueIDs, fields);

                progress.SetStatus("Loading Epics (Step 4 of 4)...");
                List<string> epics = issues.Values.Select(v => v.Fields.Epic).Where(v => !String.IsNullOrWhiteSpace(v))
                    .Union(parentIssues.Values.Select(v => v.Fields.Epic).Where(v => !String.IsNullOrWhiteSpace(v)))
                    .Distinct().ToList();
                Dictionary<string, JiraIssue> pulledEpics = GetIssues(progress, epics.ToList(), fields, (issue) => issue.Key);
                foreach (string epicId in epics)
                {
                    if (!pulledEpics.TryGetValue(epicId, out JiraIssue epic))
                    {
                        continue;
                    }

                    issues[epicId] = epic;
                }

                progress.SetStatus("Formatting Report...");
                List<CapExReportData> data = new List<CapExReportData>();
                foreach (JiraWorklog worklogItem in worklogItems)
                {
                    if (!issues.TryGetValue(worklogItem.IssueID, out JiraIssue issue))
                        throw new Exception("");

                    JiraIssue epic = null;

                    if (!String.IsNullOrWhiteSpace(issue.Fields.Epic))
                    {
                        issues.TryGetValue(issue.Fields.Epic, out epic);
                    }

                    if (epic == null && issue.Fields.Parent != null)
                    {
                        if (parentIssues.TryGetValue(issue.Fields.Parent.ID, out JiraIssue parentIssue))
                        {
                            if (!String.IsNullOrWhiteSpace(parentIssue.Fields.Epic))
                            {
                                issues.TryGetValue(parentIssue.Fields.Epic, out epic);
                            }

                        }
                    }

                    CapExReportData item = new CapExReportData();

                    item.IssueId = issue.Key;
                    item.EpicIssueId = epic?.Key;
                    item.Summary = issue.Fields.Summary;
                    item.EpicSummary = epic?.Fields?.Summary;
                    item.Capex = epic?.Fields?.CapEx?.FirstOrDefault()?.Value == "Yes" ? true : false;
                    item.IssueType = issue.Fields.IssueType?.Name;
                    item.OriginalEstimateHours = issue.OriginalEstimateHours?.ToString() ?? string.Empty;
                    item.ResolutionDate = issue.Fields.ResolutionDate;
                    item.Resolution = issue.Fields.Resolution?.Name;
                    item.LoggedBy = worklogItem.Author.DisplayName;
                    item.ProjectName = issue.Fields.Project?.Name;
                    item.ProjectCategory = issue.Fields.Project?.ProjectCategory?.Name;
                    item.Hours = ((decimal)worklogItem.TimeSpentSeconds / 3600M);
                    item.IssueCreatedDate = issue.Fields.Created.ToString();
                    item.StartDate = worklogItem.Started.ToString();
                    item.LoggedOn = worklogItem.Created.ToString();
                    item.Components = String.Join(",", issue.Fields.Components?.Select(c => c.Name) ?? Enumerable.Empty<string>());
                    item.FixVersions = String.Join(",", issue.Fields.FixVersions?.Select(v => v.Name) ?? Enumerable.Empty<string>());
                    item.TaxCredit = issue.Fields.Qualified?.Value == "Qualified" ? "Qualified" : "Not-Qualified";

                    data.Add(item);
                }

                trackingDisplay.SetNextModel("Data", data);

                progress.SetStatus("Done.");
                progress.Complete();
            }

            private Dictionary<string, JiraIssue> GetIssues(IReportProgress progress, List<string> issueIDs, string[] fields,
                Func<JiraIssue, string> idSelector = null)
            {
                idSelector = idSelector = idSelector ?? ((JiraIssue jiraIssue) => jiraIssue.ID);

                int issuesPerRequest = 100;
                progress.ClearReportProgress();
                progress.SetReportTotal((int)Math.Ceiling((decimal)issueIDs.Count / issuesPerRequest));

                List<JiraIssue> issues = new List<JiraIssue>();
                for (int i = 0; i < issueIDs.Count; i += issuesPerRequest)
                {
                    issues.AddRange(this.jiraIssueService.GetIssues(issueIDs.Skip(i).Take(issuesPerRequest).ToList(), fields));
                    progress.IncremenetReportProgress();
                }

                return issues.ToDictionary(idSelector, c => c);
            }

            private int GetTotalWeekdays(DateTime startDate, DateTime endDate)
            {
                int totalWeekdays = 0;
                var start = startDate.Date;
                var end = endDate.Date;

                while (start < end)
                {
                    if (start.DayOfWeek != DayOfWeek.Saturday && start.DayOfWeek != DayOfWeek.Sunday)
                    {
                        totalWeekdays++;
                    }

                    start = start.AddDays(1);
                }

                return totalWeekdays;
            }

            private List<JiraWorklog> GetWorklogs(IReportProgress progress, DateTime startDate, DateTime endDate)
            {
                DateTime startDateIndex = startDate;
                DateTime endDateIndex;

                List<JiraWorklog> worklog = new List<JiraWorklog>();
                progress.ClearReportProgress();
                progress.SetReportTotal((int)Math.Ceiling((endDate - startDate).TotalDays / 7));

                do
                {
                    endDateIndex = startDateIndex.AddDays(7);
                    if (endDateIndex > endDate)
                        endDateIndex = endDate;

                    worklog.AddRange(this.worklogService.GetUpdatedWorklogs(startDateIndex, endDateIndex));

                    startDateIndex = endDateIndex;

                    progress.IncremenetReportProgress();
                } while (endDateIndex != endDate);

                return worklog;
            }
        }
    }
}
