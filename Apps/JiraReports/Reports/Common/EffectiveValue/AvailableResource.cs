using JiraReports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.Common.EffectiveValue
{
    public class AvailableResource
    {

        public string Name { get; set; }

        public decimal Availability { get; set; }

        public decimal ProjectedHours { get; set; }

        public PersonRole Role { get; set; }

    }
}
