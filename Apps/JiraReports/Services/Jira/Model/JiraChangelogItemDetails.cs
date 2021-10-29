using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira.Model
{
    public class JiraChangelogItemDetails
    {
        public string Field { get; set; }
        public string FieldType { get; set; }
        public string From { get; set; }
        public string FromString { get; set; }
        public string To { get; set; }
        [JsonProperty("toString")]
        public string ToStr { get; set; }
    }
}
