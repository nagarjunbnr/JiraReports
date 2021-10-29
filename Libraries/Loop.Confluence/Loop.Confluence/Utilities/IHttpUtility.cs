using System;
using System.Collections.Generic;
using System.Text;

namespace Loop.Confluence.Utilities
{
    public interface IHttpUtility
    {
        string UrlEncode(string url);
        string Combine(params string[] paths);
    }
}
