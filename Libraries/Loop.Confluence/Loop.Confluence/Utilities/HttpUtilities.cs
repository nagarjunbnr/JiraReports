using Loop.Confluence.IoC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Loop.Confluence.Utilities
{
    [SingleInstance(typeof(IHttpUtility))]
    public class HttpUtilities : IHttpUtility
    {
        public string UrlEncode(string url)
        {
            return HttpUtility.UrlEncode(url);
        }

        public string Combine(params string[] paths)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < paths.Length; i++)
            {
                string currentPath = paths[i].Trim();
                string previousPath = i > 0 ? paths[i - 1].Trim() : String.Empty;

                if (i > 0 && previousPath.EndsWith("/") && currentPath.StartsWith("/"))
                {
                    builder.Append(currentPath.Substring(1));
                    continue;
                }

                if (i > 0 && !previousPath.EndsWith("/") && !currentPath.StartsWith("/"))
                {
                    builder.Append("/");
                }

                builder.Append(currentPath);
            }

            return builder.ToString();
        }
    }
}
