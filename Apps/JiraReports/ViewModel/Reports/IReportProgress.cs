using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.ViewModel.Reports
{
    public interface IReportProgress
    {
        event EventHandler ReportCompleted;

        void SetReportTotal(int value);

        void ClearReportProgress();

        void IncremenetReportProgress(int? value = null);

        void Complete();

        void SetStatus(string status);
    }
}
