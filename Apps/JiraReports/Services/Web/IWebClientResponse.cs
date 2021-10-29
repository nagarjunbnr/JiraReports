using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Web
{
    public interface IWebClientResponse : IDisposable
    {
        HttpStatusCode StatusCode { get; }
        Stream GetStream();
    }
}
