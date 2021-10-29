using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JiraReports
{
    public partial class Theme
    {
        private void PasswordSource_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // PasswordBoxes don't let you bind on the Password field, so we have to update the parent
            // when the template's password has been changed. Also we are updating a hidden textfield here
            // so that we can fire a trigger to remove the placeholder when the password is not blank.
            // SO that the password is not stored in plaintext, we are just setting the "hidden" password
            // to a single character when the template's password is not empty.
            PasswordBox PasswordSource = e.Source as PasswordBox;
            Label HiddenPassword = PasswordSource.Template.FindName("HiddenPassword", PasswordSource) as Label;
            HiddenPassword.Content = String.IsNullOrWhiteSpace(PasswordSource.Password) ? "" : "*";
        }

        protected void MaterialButton_Loaded(object sender, RoutedEventArgs e)
        {
            Grid src = e.Source as Grid;
            src.Width = src.ActualWidth;
            src.Height = src.ActualHeight;
        }
    }
}
