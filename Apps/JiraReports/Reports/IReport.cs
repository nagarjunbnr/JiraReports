using JiraReports.ViewModel.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JiraReports.Reports
{
    public interface IReport
    {
        string Name { get; }

        bool IsVisible { get; }

        void RunReport(ReportOptionsModel options, ReportDisplayModel display, IReportProgress progress);
    }
}
