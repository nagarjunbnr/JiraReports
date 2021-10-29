using System;
using System.Collections.Generic;
using System.Text;

namespace Loop.Confluence.Utilities
{
    public interface IWebClient
    {
        IWebClientResponse Get(IWebAuthentication auth, string url, params (string key, string value)[] qs);
        IWebClientResponse Post(IWebAuthentication auth, string url, object json);
        IWebClientResponse Put(IWebAuthentication auth, string url, object json);
    }
}
