using JiraReports.Services;
using JiraReports.Services.Jira;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.ViewModel.Reports.TimeTracking
{
    [InstancePerRequest]
    public class TimeTrackingOptionsViewModel : ReportOptionsModel
    {
        private DateTime? _startDate;
        private DateTime? _endDate;


        public TimeTrackingOptionsViewModel(IJiraProjectService projectService)
        {
            this.SelectableProjectCategories = projectService.GetProjectCategories().Select(c => new SelectableProjectCategory()
            {
                Category = c
            }).ToList();
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

        public List<SelectableProjectCategory> SelectableProjectCategories
        {
            get; set;
        }


        public override bool Validate()
        {
            return this.StartDate != null;
        }
    }
}
