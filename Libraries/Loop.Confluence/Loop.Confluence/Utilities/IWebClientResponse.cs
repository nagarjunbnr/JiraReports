using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Loop.Confluence.Utilities
{
    public interface IWebClientResponse : IDisposable
    {
        HttpStatusCode StatusCode { get; }
        IEnumerable<Cookie> Cookies { get; }
        Stream GetStream();
    }
}
