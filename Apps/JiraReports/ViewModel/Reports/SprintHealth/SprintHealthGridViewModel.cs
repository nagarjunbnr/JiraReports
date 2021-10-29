using JiraReports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.ViewModel.Reports.SprintHealth
{
    [InstancePerRequest]
    public class SprintHealthGridViewModel : MultiGridViewModel
    {

        public SprintHealthGridViewModel(IServiceLocator serviceLocator) : base(serviceLocator)
        {

        }

    }
}
