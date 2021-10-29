using JiraReports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.ViewModel.Reports.SprintPointValue
{
    [InstancePerRequest]
    public class SprintPointValueGridViewModel : MultiGridViewModel
    {
        public SprintPointValueGridViewModel(IServiceLocator serviceLocator) : base(serviceLocator)
        {

        }
    }
}
