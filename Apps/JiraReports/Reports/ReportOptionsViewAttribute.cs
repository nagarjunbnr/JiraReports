using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ReportOptionsViewAttribute : Attribute
    {
        public Type OptionsViewType { get; private set; }
        public Type OptionsViewModelType { get; private set; }

        public ReportOptionsViewAttribute(Type optionsViewType, Type optionsViewModelType)
        {
            this.OptionsViewType = optionsViewType;
            this.OptionsViewModelType = optionsViewModelType;
        }
    }
}
