using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Web
{
    public interface IWebAuthentication
    {
        IEnumerable<(string key, string value)> GenerateAuthHeaders();
    }
}
