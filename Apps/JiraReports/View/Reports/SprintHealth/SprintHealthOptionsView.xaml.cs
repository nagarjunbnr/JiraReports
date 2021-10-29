using JiraReports.Services;
using JiraReports.Services.Jira;
using JiraReports.ViewModel.Reports;
using JiraReports.ViewModel.Reports.SprintHealth;
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

namespace JiraReports.View.Reports.SprintHealth
{
    /// <summary>
    /// Interaction logic for BlankOptionsView.xaml
    /// </summary>
    [InstancePerRequest]
    public partial class SprintHealthOptionsView : UserControl
    {
        public SprintHealthOptionsView()
        {
            InitializeComponent();
        }
    }
}
