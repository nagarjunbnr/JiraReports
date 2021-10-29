using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira.Model
{
    public class AgileServiceResult<ResultType>
    {
        public int MaxResults { get; set; }
        public int StartsAt { get; set; }
        public bool IsLast { get; set; }
        public List<ResultType> Values { get; set; }
    }

}