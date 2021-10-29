using JiraReports.Reports;
using JiraReports.Services;
using JiraReports.ViewModel.Reports;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JiraReports.ViewModel.ReportPage
{
    [InstancePerRequest]
    public class ReportPageViewModel : ViewModel
    {
        private int selectedIndex;

        private IServiceLocator serviceLocator;
        private IEnumerable<IReport> reportTypes;

        public ObservableCollection<ReportViewModel> Reports { get; set; }
        public ObservableCollection<ReportTypeViewModel> ReportTypes { get; set; }

        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                selectedIndex = value;
                RaisePropertyChangedEvent("SelectedIndex");
            }
        }

        private void Reports_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                this.SelectedIndex = this.Reports.Count - 1;
            }
        }

        public ReportPageViewModel(IEnumerable<IReport> reportTypes, IServiceLocator serviceLocator)
        {
            this.Reports = new ObservableCollection<ReportViewModel>();
            this.ReportTypes = new ObservableCollection<ReportTypeViewModel>();
            this.Reports.CollectionChanged += Reports_CollectionChanged;

            this.reportTypes = reportTypes;
            this.serviceLocator = serviceLocator;

            foreach(IReport report in reportTypes)
            {
                ReportTypes.Add(serviceLocator.GetObject<ReportTypeViewModel, IReport, ReportPageViewModel>(report, this));
            }
        }
    }
}
