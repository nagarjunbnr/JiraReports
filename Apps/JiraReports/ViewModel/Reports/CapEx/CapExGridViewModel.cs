using JiraReports.Reports.TimeTracking;
using JiraReports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.ViewModel.Reports.CapEx
{
    [InstancePerRequest]
    public class CapExGridViewModel : MultiGridViewModel
    {
        public CapExGridViewModel(IServiceLocator serviceLocator) : base(serviceLocator)
        {

        }
    }
}
