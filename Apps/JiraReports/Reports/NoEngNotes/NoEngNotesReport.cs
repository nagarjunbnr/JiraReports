using JiraReports.Common;
using JiraReports.Services;
using JiraReports.Services.Jira;
using JiraReports.Services.Jira.Model;
using JiraReports.View.Reports;
using JiraReports.View.Reports.TimeTracking;
using JiraReports.ViewModel.Reports;
using JiraReports.ViewModel.Reports.NoEngNotes;
using JiraReports.ViewModel.Reports.TimeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace JiraReports.Reports.NoEngNotes
{
    [SingleInstance(typeof(IReport))]
    [ReportOptionsView(optionsViewType: typeof(BlankOptionsView), optionsViewModelType: typeof(BlankOptionsViewModel))]
    [ReportDisplayView(displayViewType: typeof(ReportMultiGridView), displayViewModelType: typeof(NoEngNotesGridViewModel))]
    public class NoEngNotesReport : IReport
    {
        private IJiraAgileService agileService;
        private IJiraGitService jiraGitService;
        private JiraConstants jiraConstants;

        private string[] issueTypes = new string[]
        {
            "Bug", "Feature"
        };

        private string[] statuses = new string[]
        {
            "In Review", "Testing", "Done"
        };

        public string Name => "No Eng. Notes";

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


        public NoEngNotesReport(IJiraAgileService agileService, IJiraGitService jiraGitService, JiraConstants jiraConstants)
        {
            this.agileService = agileService;
            this.jiraGitService = jiraGitService;
            this.jiraConstants = jiraConstants;
        }

        public void RunReport(ReportOptionsModel options, ReportDisplayModel display, IReportProgress progress)
        {
            NoEngNotesGridViewModel engNotesDisplay = display as NoEngNotesGridViewModel;
            progress.ClearReportProgress();
            engNotesDisplay.ClearModels();

            progress.SetStatus("Getting a list of boards (Step 1 of 3)...");
            List<AgileBoard> relevantBoards = agileService.GetAllBoards()
                .Where(b => this.jiraConstants.MainBoards.Any(bn => bn == b.Name))
                .ToList();

            progress.SetStatus("Getting Active Sprints (Step 2 of 3)...");
            progress.ClearReportProgress();
            progress.SetReportTotal(relevantBoards.Count);
            List<AgileSprint> activeSprints = new List<AgileSprint>();
            foreach(AgileBoard board in relevantBoards)
            {
                activeSprints.AddRange(agileService.GetSprintsForBoard(board.ID).Where(s => s.State == "active"));
                progress.IncremenetReportProgress();
            }

            progress.SetStatus("Getting Cases in Sprints (Step 3 of 3)...");
            progress.ClearReportProgress();
            progress.SetReportTotal(activeSprints.Count);
            List<JiraIssue> issuesWithNoNotes = new List<JiraIssue>();
            Dictionary<string, List<GitCommit>> commitsForCases = new Dictionary<string, List<GitCommit>>();
            Dictionary<string, AgileSprint> sprintsForCases = new Dictionary<string, AgileSprint>();
            List<JiraIssue> allIssuesWithEngNotes = new List<JiraIssue>();
            foreach (AgileSprint sprint in activeSprints)
            {
                List<JiraIssue> allIssuesInSprint = agileService.GetIssuesForSprint(sprint.ID, "issuetype", "customfield_11200", "status");
                List<JiraIssue> possibleIssues = allIssuesInSprint
                    .Where(i => String.IsNullOrWhiteSpace(i.Fields?.EngNotesForQA))
                    .Where(i => issueTypes.Any(it => it == i.Fields?.IssueType?.Name))
                    .Where(i => statuses.Any(st => st == i.Fields?.Status?.Name))
                    .ToList();

                allIssuesWithEngNotes.AddRange(allIssuesInSprint
                    .Where(i => !String.IsNullOrWhiteSpace(i.Fields?.EngNotesForQA)));

                foreach(JiraIssue issue in allIssuesInSprint)
                {
                    sprintsForCases[issue.Key] = sprint;
                }

                foreach (JiraIssue issue in possibleIssues)
                {
                    List<GitCommit> commits = this.jiraGitService.GetIssueCommits(issue.Key);

                    if(commits != null && commits.Count > 0)
                    {
                        commitsForCases[issue.Key] = commits;
                        issuesWithNoNotes.Add(issue);
                    }
                }

                progress.IncremenetReportProgress();
            }

            progress.SetStatus("Formatting Report...");
            List<NoNotesSummaryDataItem> data = issuesWithNoNotes
                .Select(i => new NoNotesSummaryDataItem()
                {
                    Sprint = sprintsForCases[i.Key].Name,
                    Engineer = commitsForCases[i.Key].FirstOrDefault().Author,
                    Issue = i.Key,
                    Status = i.Fields.Status.Name
                })
                .ToList();
            engNotesDisplay.SetNextModel("Detailed", data);

            List<NoNotesBySprintDataItem> bySprint = issuesWithNoNotes
                .GroupBy(i => sprintsForCases[i.Key].Name)
                .Select(i => new NoNotesBySprintDataItem()
                {
                    Sprint = i.Key,
                    Count = i.Count()
                })
                .ToList();
            engNotesDisplay.SetNextModel("By Sprint", bySprint);

            List<NoNotesByEngineerDataItem> byEngineer = issuesWithNoNotes
                .GroupBy(i => commitsForCases[i.Key].FirstOrDefault().Author)
                .Select(i => new NoNotesByEngineerDataItem()
                {
                    Engineer = i.Key,
                    Count = i.Count()
                })
                .ToList();
            engNotesDisplay.SetNextModel("By Engineer", byEngineer);

            List<AllEngNotesForQADataItem> allNotes = allIssuesWithEngNotes
                .Select(i => new AllEngNotesForQADataItem()
                {
                    Issue = i.Key,
                    Status = i.Fields.Status.Name,
                    Sprint = sprintsForCases[i.Key].Name,
                    EngNotes = i.Fields.EngNotesForQA
                })
                .ToList();
            engNotesDisplay.SetNextModel("All ENG Notes", allNotes);


            progress.SetStatus("Done.");
            progress.Complete();
        }
    }
}
