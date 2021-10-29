using JiraReports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.SprintHealth
{
    public class SprintHealthPersonalStats
    {

        private decimal _loggedHours;

        public string Name { get; set; }

        public PersonRole Role { get; set; }

        public decimal? ProjectedHours { get; set; }

        public decimal LoggedHours
        {
            set
            {
                _loggedHours = value;
            }
            get
            {
                return Math.Round(_loggedHours, 2);
            }
        }

    }
}
