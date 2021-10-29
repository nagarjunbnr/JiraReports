using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    public class JiraEpicField
    {
        public string Summary { get; set; }

        [JsonProperty("customfield_10100")]
        public string EpicCaseTag { get; set; }

        [JsonProperty("customfield_12509")]
        public InvestmentCategory InvestmentCategory { get; set; }
    }

    public class InvestmentCategory { 
        [JsonProperty("value")]
        public string Category { get; set; }
    }
}
