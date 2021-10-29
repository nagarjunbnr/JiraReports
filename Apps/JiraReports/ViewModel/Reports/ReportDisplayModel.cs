using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JiraReports.ViewModel.Reports
{
    public abstract class ReportDisplayModel : ViewModel, IReportProgress
    {
        private int reportProgress = 0;
        private int reportTotal = 1;
        private string reportStatus;

        public event EventHandler ReportCompleted;

        public int ReportProgress
        {
            get => reportProgress;
            set
            {
                reportProgress = value;
                RaisePropertyChangedEvent("ReportProgress");
            }
        }

        public int ReportTotal
        {
            get => reportTotal;
            set
            {
                reportTotal = value;
                RaisePropertyChangedEvent("ReportTotal");
            }
        }

        public string ReportStatus
        {
            get => reportStatus;
            set
            {
                reportStatus = value;
                RaisePropertyChangedEvent("ReportStatus");
            }
        }

        public void SetReportTotal(int value)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                this.ReportTotal = value;
            });
        }

        public void ClearReportProgress()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                this.ReportProgress = 0;
            });
        }

        public void IncremenetReportProgress(int? value = null)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                this.ReportProgress += (value ?? 1);
            });
        }

        public void SetStatus(string status)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                this.ReportStatus = status;
            });
        }

        public void Complete()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                if (ReportCompleted != null)
                {
                    ReportCompleted(this, null);
                }
                this.ReportProgress = 0;
            });
        }
    }
}
