using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Common
{
    public static class LinqExtensions
    {
        public static IEnumerable<object> ToObjectEnumerable(this IEnumerable enumerable)
        {
            foreach(object item in enumerable)
            {
                yield return item;
            }
        }
    }
}
