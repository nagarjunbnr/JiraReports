using JiraReports.Reports;
using JiraReports.Services;
using JiraReports.ViewModel.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.ViewModel.ReportPage
{
    [InstancePerRequest]
    public class ReportTypeViewModel : ViewModel
    {
        private IServiceLocator serviceLocator;
        private ReportPageViewModel reportPageViewModel;

        public Type ReportType => this.Report.GetType();
        public string Name => this.Report.Name;
        public bool IsVisible => this.Report.IsVisible;
        public IReport Report { get; }

        public ViewModelCommand NewReportCommand { get; set; }

        public ReportTypeViewModel(IServiceLocator serviceLocator, ReportPageViewModel reportPageViewModel, IReport report)
        {
            this.serviceLocator = serviceLocator;
            this.reportPageViewModel = reportPageViewModel;

            this.Report = report; 
            this.NewReportCommand = new ViewModelCommand(NewReport, () => true);
        }

        private void NewReport()
        {
            reportPageViewModel.Reports.Add(serviceLocator.GetObject<ReportViewModel, IReport, ReportPageViewModel>(this.Report, reportPageViewModel));
        }
    }
}
