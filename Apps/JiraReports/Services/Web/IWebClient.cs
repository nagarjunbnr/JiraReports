using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Web
{
    public interface IWebClient
    {
        IWebAuthentication Authentication { get; set; }
        IWebClientResponse Get(string url, params (string key, string value)[] qs);
        IWebClientResponse Post(string url, object json);
    }
}
