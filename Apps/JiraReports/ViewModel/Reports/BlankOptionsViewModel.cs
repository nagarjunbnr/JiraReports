using JiraReports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.ViewModel.Reports
{
    [InstancePerRequest]
    public class BlankOptionsViewModel : ReportOptionsModel
    {
        public override bool Validate() => true;
    }
}
