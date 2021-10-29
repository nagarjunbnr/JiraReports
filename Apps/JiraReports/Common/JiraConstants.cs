using JiraReports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Common
{
    [SingleInstance]
    public class JiraConstants
    {
        public static string EngineerRoleFriendlyName = "Engineer";
        public static string QARoleFriendlyName = "Quiality Assurance";

        public static string IncompleteReason_Time = "Not enough time";
        public static string IncompleteReason_Scope = "Scope broader than refined";
        public static string IncompleteReason_Requirements = "Requirements were unclear";
        public static string IncompleteReason_PMO = "PMO priority change";
        public static string IncompleteReason_WrongTeam = "Incorrect delivery team";
        public static string IncompleteReason_ThirdParty = "Waiting on 3rd Party";

        public static int MaxWorklogsPerIssue = 20;

        public string[] MainBoards { get; } = new string[]
        {
            "BOOK", "Quote", "Integration", "Campaigns", "Mobile", "MPI", "Reporting",
            "SmartLane", "Triggers", "Loop 5.0", "TIV", "SLT", "LSEC", 
            "Essentials", "Affinitiv Core","DL board", "XRM Core", "XRM Integration", "COE Board", "QA Automation",
            "ADMT Sprints", "TimeHighway Scrum"
            //"Mike Test", "XRM Core", "XRM Integration"
        };

        public static string[] MainFields = new string[] {
                    "project",
                    "status",
                    "summary",
                    "components",
                    "customfield_10104", //Sprint
                    "customfield_10313", // OriginalQAHours
                    "customfield_10312", // OriginalEngHours
                    "customfield_10106", // StoryPoints
                    "customfield_12000", //Incomplete Reason
                    "customfield_11003", //ZendeskIssueCount
                    "issuetype",
                    "resolution",
                    "resolutiondate",
                    "worklog",
                    "timeoriginalestimate",
                    "parent",
                    "created",
                    "issuelinks",
                    "customfield_10100",
                    "customfield_11800",
                    "customfield_12000"
        };

        public JiraProjectItems[] Projects { get; } = new JiraProjectItems[]
        {
            new JiraProjectItems("Integration", "IN", "Integration", "Integration"),
            new JiraProjectItems("Mobile", "MOB", "Mobile", "Mobile"),
            new JiraProjectItems("Quote", "QUOTE", "Quote", "Quote"),
            new JiraProjectItems("Reporting", "REP", "Reporting", "Reporting"),
            new JiraProjectItems("Service Lane Technology", "SLT", "SLT", "SLT"),
            new JiraProjectItems("TIV", "TIV", "TIV", "TIV"),
            new JiraProjectItems("XRM Core", "XC", "XRMCore", "XRM Core"),
            new JiraProjectItems("XRM Integration", "XI", "XRMInt", "XRM Integration"),
            new JiraProjectItems("Essentials", "ESS", "Essentials", "Essentials"),
            new JiraProjectItems("Affinitiv Core", "AFC", "Affinitiv Core", "Affinitiv Core"),
            new JiraProjectItems("DPS Legacy","DL", "DPS", "DL board"),
            new JiraProjectItems("Center of Excellence for Reporting","COE", "COE Reporting", "COE Board"),
            new JiraProjectItems("QA Automation","QAA", "QA Automation", "QA Automation"),
            new JiraProjectItems("ADMT Engineering","ADMT", "ADMT", "ADMT Sprints"),
            new JiraProjectItems("Time Highway","THY", "TH", "TimeHighway Scrum")
            //new JiraProjectItems("Mike Test", "MTEST", "Mike Test", "Mike Test"),
        };


        public class SprintNames
        {
            public static string Campaigns = "Campaign";
            public static string Integration = "Integration";
            public static string Loop5 = "L5";
            public static string LoopSecurity = "LSEC";
            public static string Mobile = "Mobile";
            public static string Quote = "Quote";
            public static string Reporting = "Reporting";
            public static string ServiceLaneTechnology = "SLT";
            public static string TradeInValet = "TiV";
            public static string Triggers = "Triggers";
            public static string XRMCore = "XRM Core";
            public static string XRMIntegration = "XRM Integration";
            public static string Essentials = "Essentials";
            public static string Core = "Affinitiv Core";
            public static string DPS = "DL board";
            public static string COE = "Center of Excellence for Reporting";
            public static string QAA = "QA Automation";
            public static string ADMT = "ADMT Sprints";
            public static string TH = "TimeHighway Scrum";
        }

        public class ProjectKeys
        {
            public static string AffinitivCore = "AFC";
            public static string Campaigns = "CAM";
            public static string Integration = "IN";
            public static string Loop5 = "L5";
            public static string LoopSecurity = "LSEC";
            public static string Mobile = "MOB";
            public static string Quote = "QUOTE";
            public static string Reporting = "REP";
            public static string ServiceLaneTechnology = "SLT";
            public static string TradeInValet = "TIV";
            public static string Triggers = "TRIG";
            public static string XRMCore = "XC";
            public static string XRMIntegration = "XI";
            public static string Essentials = "ESS";
            public static string Core = "AFC";
            public static string Architecture = "ARCH";
            public static string DPSLegacy = "DL";
            public static string COEReporting = "COE";
            public static string QAAutomation = "QAA";
            public static string ADMTEngineering = "ADMT";
            public static string TimeHighway = "THY";
        }
    }

    public class JiraProjectItems
    { 
        public string FriendlyName { get; set; }
        public string Key { get; set; }
        public string SprintName { get; set; }
        public string BoardName { get; set; }

        public JiraProjectItems(string friendlyName, string key, string sprintName, string boardName)
        {
            this.FriendlyName = friendlyName;
            this.Key = key;
            this.SprintName = sprintName;
            this.BoardName = boardName;
        }
    }

   

}
