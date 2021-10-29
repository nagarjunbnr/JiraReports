using JiraReports.Reports.TimeTracking;
using JiraReports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.ViewModel.Reports.NoEngNotes
{
    [InstancePerRequest]
    public class NoEngNotesGridViewModel : MultiGridViewModel
    {
        public NoEngNotesGridViewModel(IServiceLocator serviceLocator) : base(serviceLocator)
        {

        }
    }
}
