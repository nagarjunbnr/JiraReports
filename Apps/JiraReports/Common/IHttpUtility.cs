using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Common
{
    public interface IHttpUtility
    {
        string UrlEncode(string url);
        string Combine(params string[] paths);
    }
}
