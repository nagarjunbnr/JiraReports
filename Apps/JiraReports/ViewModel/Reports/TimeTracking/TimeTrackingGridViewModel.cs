using JiraReports.Reports.TimeTracking;
using JiraReports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.ViewModel.Reports.TimeTracking
{
    [InstancePerRequest]
    public class TimeTrackingGridViewModel : MultiGridViewModel
    {
        public TimeTrackingGridViewModel(IServiceLocator serviceLocator) : base(serviceLocator)
        {

        }
    }
}
