using JiraReports.Services;
using JiraReports.ViewModel.ReportPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JiraReports.View
{
    /// <summary>
    /// Interaction logic for ReportPage.xaml
    /// </summary>
    [InstancePerRequest, View("Report", typeof(ReportPageViewModel))]
    public partial class ReportPage : UserControl
    {
        public ReportPage()
        {
            InitializeComponent();
        }
    }
}
