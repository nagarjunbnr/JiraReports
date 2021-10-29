using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Model
{
    public class SprintParameters
    {
        public string SprintNumber { get; set; }

        public DayOfWeek SprintPlanningDay { get; set; }

        public DayOfWeek SprintReleaseDay { get; set; }

        public DayOfWeek SprintRetrospectiveDay { get; set; }

        public bool IsPlanningBeforeRelease { get; set; }

        //sprint ends on either retro day or release day, whichever comes last
        public DayOfWeek SprintEndDay
        {
            get
            {
                return (DayOfWeek)Math.Max((int)this.SprintReleaseDay, (int)this.SprintRetrospectiveDay);
            }
        }

    }
}
