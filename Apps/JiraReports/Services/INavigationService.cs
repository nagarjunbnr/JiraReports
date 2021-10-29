using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services
{
    public interface INavigationService
    {
        void StartContainer();
        void Navigate(string path);
    }
}
