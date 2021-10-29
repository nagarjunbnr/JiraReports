using JiraReports.Common;
using JiraReports.Services.Jira.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    public class JiraFields
    {
        public List<JiraFixVersion> FixVersions { get; set; }
        //public string Resolution { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime LastViewed { get; set; }
        public JiraPriority Priority { get; set; }
        //public List<string> Labels { get; set; }
        public List<JiraIssueLink> IssueLinks { get; set; } = new List<JiraIssueLink>();
        public string Summary { get; set; }
        public JiraUserBasic Assignee { get; set; }
        public JiraUserBasic Creator { get; set; }
        public JiraUserBasic Reporter { get; set; }
        public JiraIssueType IssueType { get; set; }
        public JiraIssueStatus Status { get; set; }
        public JiraIssueResolution Resolution { get; set; }
        public string ResolutionDate { get; set; }
        public object OriginalEstimate { get; set; }
        public JiraProject Project { get; set; }
        public JiraIssue Parent { get; set; }
        [JsonProperty("timeoriginalestimate")]
        public string TimeOriginalEstimate { get; set; }

        public JiraTimeTrackingData TimeTracking { get; set; }

        public List<JiraIssueComponent> Components { get; set; } = new List<JiraIssueComponent>();

        public JiraWorklogList Worklog { get; set; }

        [JsonProperty("customfield_10302")]
        public JiraCustomFieldValue Pilot { get; set; }

        [JsonProperty("customfield_10102")]
        public string EpicName { get; set; }

        [JsonProperty("customfield_10101")]
        public JiraCustomFieldValue EpicStatus { get; set; }

        [JsonProperty("customfield_10310")]
        public string Client { get; set; }

        [JsonProperty("customfield_10313")]
        public string OriginalQAHours { get; set; }

        [JsonProperty("customfield_10312")]
        public string OriginalEngHours { get; set; }

        [JsonProperty("customfield_10106")]
        public string StoryPoints { get; set; }

        [JsonProperty("customfield_10307")]
        public string ReleaseNotes { get; set; }

        [JsonProperty("customfield_10604")]
        public string FinalTestNotes { get; set; }

        [JsonProperty("customfield_10406")]
        public List<string> Branches { get; set; }

        [JsonProperty("customfield_10304")]
        public JiraCustomFieldValue Qualified { get; set; }

        [JsonProperty("customfield_11500")]
        public List<JiraCustomFieldValue> CapEx { get; set; }

        [JsonConverter(typeof(JiraFieldsSprintConverter))]
        [JsonProperty("customfield_10104")]
        public string Sprint { get; set; }

        [JsonProperty("customfield_10100")]
        public string Epic { get; set; }

        [JsonProperty("customfield_11200")]
        public string EngNotesForQA { get; set; }

        [JsonProperty("customfield_11003")]
        public int? ZendeskIssueCount { get; set; }

        [JsonProperty("customfield_12000")]
        public JiraCustomFieldValue IncompleteReason { get; set; }

        public Dictionary<string, string> CustomFields { get; set; } = new Dictionary<string, string>();
    }
}
