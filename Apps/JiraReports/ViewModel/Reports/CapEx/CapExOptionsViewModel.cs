using JiraReports.Services;
using JiraReports.Services.Jira;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.ViewModel.Reports.CapEx
{
    [InstancePerRequest]
    public class CapExOptionsViewModel : ReportOptionsModel
    {
        private DateTime? _startDate;
        private DateTime? _endDate;


        public CapExOptionsViewModel(IJiraProjectService projectService)
        {

        }

        public DateTime? StartDate
        {
            get => this._startDate;
            set
            {
                this._startDate = value;
                RaisePropertyChangedEvent("StartDate");
            }
        }

        public DateTime? EndDate
        {
            get => this._endDate;
            set
            {
                this._endDate = value;
                RaisePropertyChangedEvent("EndDate");
            }
        }


        public override bool Validate()
        {
            return this.StartDate != null;
        }
    }
}
