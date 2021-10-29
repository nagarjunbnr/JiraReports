using System;
using System.Collections.Generic;
using System.Text;

namespace Loop.Confluence.Utilities
{
    public class ConfluenceConfig : IConfluenceConfig
    {
        public IWebAuthentication Authentication { get; set; }

        public string APIBaseUrl { get; set; }
    }
}
