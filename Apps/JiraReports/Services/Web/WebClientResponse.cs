using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Web
{
    public class WebClientResponse : IWebClientResponse
    {
        private HttpWebResponse _response;

        public HttpStatusCode StatusCode => _response.StatusCode;

        public WebClientResponse(HttpWebResponse response)
        {
            this._response = response;
        }

        public void Dispose()
        {
            this._response.Dispose();
        }

        public Stream GetStream()
        {
            return this._response.GetResponseStream();
        }
    }
}
