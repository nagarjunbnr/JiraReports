using JiraReports.Reports.Common.EffectiveValue.ViewModel;
using JiraReports.Services.Jira.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.Common.EffectiveValue
{
    public class TeamSprintStats
    {
        public TeamSprintStats()
        {
            this.AvailableResources = new List<AvailableResource>();
            this.IssueStats = new List<SprintStatsIssue>();
        }

        public string Team { get; set; }

        public int Sprint { get; set; }

        public List<JiraIssue> CustomerBugsOpenedDuringSprint { get; set; }

        public List<SprintStatsIssue> IssueStats { get; set; }

        public List<JiraIssue> FailureRateIssues { get; set; }

        public List<AvailableResource> AvailableResources { get; set; }

        public decimal TotalLoggedHours
        {
            get
            {
                return ValueLoggedHours + NonValueLoggedHours;
            }
        }

        public decimal ValueLoggedHours
        {
            get
            {
                return this.IssueStats.Where(i => i.IsValueAdded).Sum(i => i.LoggedHours);
            }
        }

        public decimal NonValueLoggedHours
        {
            get
            {
                return this.IssueStats.Where(i => !i.IsValueAdded).Sum(i => i.LoggedHours);
            }
        }

        public decimal FailureRateHours
        {
            get
            {
                return FailureRateIssues.Sum(i => i.OriginalEstimateHours ?? 0);
            }
        }

        public decimal ProductivityScore { get; set; }

        public decimal PredictabilityScore { get; set; }

        public decimal TotalResources
        {
            get
            {
                return this.AvailableResources.Sum(r => r.Availability);
            }
        }

        public decimal SprintEffectiveValue
        {
            get
            {
                if (this.TotalResources == 0) return 0m;

                return ((this.ValueLoggedHours - this.FailureRateHours) * (this.PredictabilityScore / 100m)) / this.TotalResources;
            }
        }

        public TeamSprintStatsView GetValueView()
        {

            return new TeamSprintStatsView()
            {
                Team = Team,
                Sprint = Sprint,
                ValueAddedLoggedHours = Math.Round(ValueLoggedHours, 2),
                TotalLoggedHours = Math.Round(TotalLoggedHours, 2),
                NonValueLoggedHours = Math.Round(NonValueLoggedHours, 2),
                FailureRateHours = Math.Round(FailureRateHours, 2),
                Predictability = PredictabilityScore,
                TotalResources = Math.Round(TotalResources, 2),
                SprintValue = Math.Round(SprintEffectiveValue, 2)
            };
        }

        public List<SprintStatsIssueView> GetEffectiveValueIssues()
        {

            return this.IssueStats.Select(i => new SprintStatsIssueView()
            {
                Issue = i.Issue.Key,
                IssueType = i.Issue.Fields?.IssueType?.Name,
                IsValueAdded = i.IsValueAdded,
                LoggedHours = i.LoggedHours,
                Summary = i.Summary,
                Sprint = i.Issue.Fields.Sprint
            }).ToList();
        }

        public List<FailureRateIssue> GetFailureRateIssues()
        {
            return this.FailureRateIssues.Select(i => new FailureRateIssue()
            {
                Issue = i.Key,
                IssueType = i.Fields.IssueType.Name,
                ZenDeskTicketCount = i.Fields.ZendeskIssueCount,
                EstimatedHours = i.OriginalEstimateHours,
                EstimatedStoryPoints = i.Fields.StoryPoints
            }).ToList();
        }

        public List<AvailableResource> GetAvailableResources()
        {
            return this.AvailableResources;
        }

    }
}
