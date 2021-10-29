using JiraReports.Services;
using JiraReports.Services.Jira;
using JiraReports.Services.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace JiraReports.ViewModel.Login
{
    [InstancePerRequest]
    public class LoginViewModel : ViewModel
    {
        private INavigationService navigationService;
        private IServiceLocator serviceLocator;


        private string _username;
        private string _password;
        private Visibility _loggingInVisible = Visibility.Hidden;
        private Visibility _invalidPasswordVisible = Visibility.Hidden;

        public ViewModelCommand LoginCommand { get; set; }

        public Visibility LoggingInVisible
        {
            get => this._loggingInVisible;
            set
            {
                this._loggingInVisible = value;
                RaisePropertyChangedEvent("LoggingInVisible");
            }
        }

        public Visibility InvalidLoginVisible
        {
            get => this._invalidPasswordVisible;
            set
            {
                this._invalidPasswordVisible = value;
                RaisePropertyChangedEvent("InvalidLoginVisible");
            }
        }

        public string Username
        {
            get => this._username;
            set
            {
                this._username = value;
                RaisePropertyChangedEvent("Username");
            }
        }

        public string Password
        {
            get => this._password;
            set
            {
                this._password = value;
                RaisePropertyChangedEvent("Password");
            }
        }

        public LoginViewModel(INavigationService navigationService, IServiceLocator serviceLocator)
        {
            this.navigationService = navigationService;
            this.serviceLocator = serviceLocator;

            this.LoginCommand = new ViewModelCommand(LoginButtonClick, () => true);
        }

        public LoginViewModel()
        {

        }

        private void LoginButtonClick()
        {
            BasicAuthentication basic = this.serviceLocator.GetObject<IWebAuthentication>() as BasicAuthentication;
            basic.Username = this.Username;
            basic.Password = this.Password;

            IJiraUserService jiraService = this.serviceLocator.GetObject<IJiraUserService, IWebAuthentication>(basic);

            try
            {
                JiraUser user = jiraService.GetMe();
            }
            catch (JiraNotAuthorizedException)
            {
                this.InvalidLoginVisible = Visibility.Visible;
                return;
            }

            this.LoggingInVisible = Visibility.Visible;
            this.InvalidLoginVisible = Visibility.Hidden;

            Thread thread = new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(2000);
                this.serviceLocator.RegisterInstance<IWebAuthentication>(basic);
                this.navigationService.Navigate("Report");
            }));

            thread.Start();
        }
    }
}
