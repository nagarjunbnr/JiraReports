using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ReportDisplayViewAttribute : Attribute
    {
        public Type DisplayViewType { get; private set; }
        public Type DisplayViewModelType { get; private set; }

        public ReportDisplayViewAttribute(Type displayViewType, Type displayViewModelType)
        {
            this.DisplayViewType = displayViewType;
            this.DisplayViewModelType = displayViewModelType;
        }
    }
}
