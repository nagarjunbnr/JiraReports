using System;
using System.Collections.Generic;
using System.Text;

namespace Loop.Confluence.Utilities
{
    public interface IWebAuthentication
    {
        IEnumerable<(string key, string value)> GenerateAuthHeaders();
    }
}
