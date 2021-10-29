using JiraReports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.ViewModel.Index
{
    [InstancePerRequest]
    public class MainWindowViewModel : ViewModel
    {
        private INavigationService navigationService;

        public MainWindowViewModel(INavigationService navigation)
        {
            this.navigationService = navigation;
        }


        public override void OnInitialize()
        {
            base.OnInitialize();

        }

        public override void OnLoad()
        {
            base.OnLoad();

            this.navigationService.Navigate("Login");
            //this.navigationService.Navigate("Report");
        }
    }
}
