using JiraReports.Common;
using JiraReports.Services.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    [InstancePerRequest(typeof(IJiraWorklogService))]
    public class JiraWorklogService : JiraService, IJiraWorklogService
    {
        public JiraWorklogService(IWebClient webClient, IWebAuthentication auth, IHttpUtility httpUtility)
            : base(webClient, auth, httpUtility)
        {

        }

        public List<JiraWorklog> GetUpdatedWorklogs(DateTime fromDate, DateTime toDate)
        {
            List<JiraWorklog> returnValue = new List<JiraWorklog>();
            WorklogIDReturnValue fragment;

            do
            {
                string json = JiraGetJSON("/worklog/updated", expectedStatusCode: HttpStatusCode.OK,
                    ("since", fromDate.ToLongUnixDateTime().ToString()));
                fragment = Deserialize<WorklogIDReturnValue>(json);

                fromDate = fragment.Until;

                long[] IDs = fragment.Values
                    .Where(v => v.UpdatedTime <= toDate)
                    .Select(v => v.WorklogID).ToArray();

                if(!IDs.Any())
                    break;
                
                json = JiraPostJSON("/worklog/list", expectedStatusCode: HttpStatusCode.OK, new
                {
                    ids = IDs
                });
                returnValue.AddRange(Deserialize<List<JiraWorklog>>(json));

            } while (fragment.Values.Count == 1000);

            return returnValue;
        }
    }

    public class WorklogIDReturnValue
    {
        public List<WorklogIDItem> Values { get; set; }
        [JsonConverter(typeof(UnixLongDateTimeConverter))]
        public DateTime Since { get; set; }
        [JsonConverter(typeof(UnixLongDateTimeConverter))]
        public DateTime Until { get; set; }
        public string Self { get; set; }
        public string NextPage { get; set; }
        public bool LastPage { get; set; }
    }

    public class WorklogIDItem
    {
        public long WorklogID { get; set; }
        [JsonConverter(typeof(UnixLongDateTimeConverter))]
        public DateTime UpdatedTime { get; set; }
    }
}
