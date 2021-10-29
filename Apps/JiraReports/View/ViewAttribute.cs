using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.View
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewAttribute : ViewAttributeBase
    {
        public string Path { get; private set; }

        public ViewAttribute(string path = null, Type viewModelType = null) : base(viewModelType)
        {
            this.Path = path;
        }
    }
}
