using JiraReports.Services;
using JiraReports.ViewModel.Login;
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
    /// Interaction logic for Login.xaml
    /// </summary>
    [InstancePerRequest, View("Login", typeof(LoginViewModel))]
    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
            PasswordBox pb = new PasswordBox();
        }
    }
}
