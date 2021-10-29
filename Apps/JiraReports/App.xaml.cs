using JiraReports.Services;
using JiraReports.View;
using JiraReports.ViewModel;
using JiraReports.ViewModel.Index;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace JiraReports
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            INavigationService navService = ServiceLocator.Instance.GetObject<INavigationService>();
            navService.StartContainer();
        }
    }
}
