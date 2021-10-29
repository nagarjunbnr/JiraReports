using JiraReports.Reports;
using JiraReports.Services;
using JiraReports.ViewModel.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace JiraReports.ViewModel.ReportPage
{
    [InstancePerRequest]
    public class ReportViewModel : ViewModel
    {
        private IReport _report;
        private ReportPageViewModel reportPageViewModel;
        private IServiceLocator serviceLocator;

        private bool _isRunning = false;
        private FrameworkElement _optionsPage;
        private ReportOptionsModel _optionsViewModel;
        private FrameworkElement _displayPage;
        private ReportDisplayModel _displayViewModel;

        private Thread reportThread;

        public string Name => _report.Name;


        public FrameworkElement OptionsPage
        {
            get => _optionsPage;
            set
            {
                _optionsPage = value;
                RaisePropertyChangedEvent("OptionsPage");
            }
        }

        public ReportOptionsModel OptionsViewModel
        {
            get => _optionsViewModel;
            set
            {
                _optionsViewModel = value;
                RaisePropertyChangedEvent("OptionsViewModel");
            }
        }

        public FrameworkElement DisplayPage
        {
            get => _displayPage;
            set
            {
                _displayPage = value;
                RaisePropertyChangedEvent("DisplayPage");
            }
        }

        public ReportDisplayModel DisplayViewModel
        {
            get => _displayViewModel;
            set
            {
                _displayViewModel = value;
                RaisePropertyChangedEvent("DisplayViewModel");
            }
        }



        public ViewModelCommand RunReportCommand { get; set; }
        public ViewModelCommand CloseReportCommand { get; set; }

        public ReportViewModel(IReport report, ReportPageViewModel reportPageViewModel, IServiceLocator serviceLocator)
        {
            this._report = report;
            this.reportPageViewModel = reportPageViewModel;
            this.serviceLocator = serviceLocator;

            CreateViewsAndModels();

            this.RunReportCommand = new ViewModelCommand(RunReport, CanRunReport);
            this.CloseReportCommand = new ViewModelCommand(CloseReport, () => true);

            this.OptionsViewModel.PropertyChanged += OptionsViewModel_PropertyChanged;
        }

        private void CreateViewsAndModels()
        {
            Type reportType = this._report.GetType();

            ReportOptionsViewAttribute optionsViewAttribute = reportType.GetCustomAttributes(true).OfType<ReportOptionsViewAttribute>().Single();
            this.OptionsPage = this.serviceLocator.GetObject(optionsViewAttribute.OptionsViewType) as FrameworkElement;
            this.OptionsViewModel = this.serviceLocator.GetObject(optionsViewAttribute.OptionsViewModelType) as ReportOptionsModel;

            ReportDisplayViewAttribute displayViewAttribute = reportType.GetCustomAttributes(true).OfType<ReportDisplayViewAttribute>().Single();
            this.DisplayPage = this.serviceLocator.GetObject(displayViewAttribute.DisplayViewType) as FrameworkElement;
            this.DisplayViewModel = this.serviceLocator.GetObject(displayViewAttribute.DisplayViewModelType) as ReportDisplayModel;
            this.DisplayViewModel.ReportCompleted += Progress_ReportCompleted;
        }

        private void CloseReport()
        {
            if(!_isRunning || MessageBox.Show("The report is running, are you sure you want to close it?", 
                "Confirm Report Close", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if(_isRunning)
                {
                    reportThread.Abort();
                }

                reportPageViewModel.Reports.Remove(this);
            }
        }

        private void Progress_ReportCompleted(object sender, EventArgs e)
        {
            _isRunning = false;
            this.RunReportCommand.OnCanExcuteChanged();
        }

        private void OptionsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RunReportCommand.OnCanExcuteChanged(); 
        }

        private void RunReport()
        {
            _isRunning = true;
            this.RunReportCommand.OnCanExcuteChanged();

            this.reportThread = new Thread(new ThreadStart(() =>
            {
                this._report.RunReport(this.OptionsViewModel, this.DisplayViewModel, this.DisplayViewModel);
            }));

            reportThread.Start();
        }

        private bool CanRunReport()
        {
            return this.OptionsViewModel.Validate() && !_isRunning;
        }


    }
}
