using System;
using System.Collections.Generic;
using System.Text;

namespace Loop.Confluence.Utilities
{
    public interface IConfluenceConfig
    {
        IWebAuthentication Authentication { get; }
        string APIBaseUrl { get; set; }
    }
}
